# Tanda — Sistema de Gestión de Pedidos por Encargo

Aplicación web de uso interno para la gestión de pedidos por encargo de la **Panadería y Repostería La Espiga** (Medellín).

El negocio recibe encargos personalizados —tortas, postres y panadería por pedido— y hoy los registra en un cuaderno. Eso produce tres problemas concretos: se aceptan fechas que no alcanzan a producirse, se comprometen más encargos de los que caben en un día, y no hay un criterio único para devolver el abono cuando un cliente cancela. Tanda convierte esas tres decisiones en reglas verificables.

> Proyecto académico — Ingeniería de Software II · Institución Universitaria Pascual Bravo

---

## Tabla de contenido

- [Qué resuelve](#qué-resuelve)
- [Reglas de negocio](#reglas-de-negocio)
- [Arquitectura](#arquitectura)
- [Tecnologías](#tecnologías)
- [Puesta en marcha](#puesta-en-marcha)
- [Base de datos y migraciones](#base-de-datos-y-migraciones)
- [Roles y acceso](#roles-y-acceso)
- [Pruebas](#pruebas)
- [Calidad de código](#calidad-de-código)
- [Flujo de trabajo](#flujo-de-trabajo)
- [Estado del desarrollo](#estado-del-desarrollo)
- [Documentación](#documentación)

---

## Qué resuelve

| Antes | Con Tanda |
|---|---|
| El encargo se anota en un cuaderno y el precio se calcula a mano | La cotización se arma seleccionando producto, tamaño y adiciones; el total se calcula solo |
| La fecha de entrega se acepta "a ojo" | Se valida contra la anticipación mínima del producto antes de cotizar |
| No se sabe cuántos encargos caben en un día | Cada fecha tiene un cupo de esfuerzo que se reserva al confirmar |
| La devolución por cancelación se negocia caso por caso | La política se aplica igual para todos, según la antelación |
| El estado del pedido vive en la memoria de quien atendió | El pedido tiene un ciclo de vida explícito y solo admite las transiciones válidas |

---

## Reglas de negocio

Las decisiones del sistema están concentradas en siete clases sin dependencias externas, dentro de `Tanda.Domain/Rules`.

| Regla | Qué decide | Clase responsable |
|---|---|---|
| RN001 | Anticipación mínima según el tipo de producto (2, 4 o 5 días) | `LeadTimeValidator` |
| RN002 | Cupo diario de producción: 20 unidades de esfuerzo | `CapacityCalculator` |
| RN003 | Abono mínimo del 50 % para confirmar un pedido | `DepositValidator` |
| RN004 | Vigencia de la cotización: 24 horas | `QuoteExpiryValidator` |
| RN005 | Cálculo del precio con tamaño, adiciones y cantidad | `OrderPriceCalculator` |
| RN007 | Devolución según antelación: 100 % con más de 3 días, 50 % hasta 24 horas antes, 0 % después | `RefundCalculator` |
| — | Transiciones permitidas del ciclo de vida del pedido | `OrderStateMachine` |

Los valores numéricos (días de anticipación, porcentaje de abono, cupo diario) no están escritos en el código: se leen de la configuración a través de `IBusinessParameters` y se validan al arrancar la aplicación. Un cambio de política no exige tocar la lógica.

---

## Arquitectura

Cuatro proyectos, con las dependencias apuntando en una sola dirección:

```
Tanda.Web ──────────┐
                    ├──► Tanda.Domain  ◄──── Tanda.Domain.Tests
Tanda.Infrastructure┘
```

| Proyecto | Contiene |
|---|---|
| `Tanda.Web` | Páginas Razor, autenticación y autorización, composición de dependencias |
| `Tanda.Infrastructure` | `DbContext`, configuraciones de EF Core, repositorios, lectura de parámetros |
| `Tanda.Domain` | Entidades, reglas de negocio y tipos de apoyo (`ValidationResult`) |
| `Tanda.Domain.Tests` | Pruebas unitarias del dominio |

**`Tanda.Domain` no tiene una sola referencia NuGet.** No conoce Entity Framework, ni ASP.NET, ni la base de datos, ni el reloj del sistema: el instante actual entra como parámetro. Esa restricción es deliberada y es la que permite probar cada regla de forma aislada, sin levantar servidor ni preparar datos.

---

## Tecnologías

| Capa | Herramienta |
|---|---|
| Framework | ASP.NET Core 9 · Razor Pages |
| Acceso a datos | Entity Framework Core 9 (code-first) |
| Motor de base de datos | SQL Server |
| Autenticación | ASP.NET Core Identity con roles |
| Pruebas | xUnit · FluentAssertions |
| Cobertura | Coverlet · ReportGenerator |
| Análisis estático | SonarQube Cloud · analizadores del SDK · `.editorconfig` |

---

## Puesta en marcha

### Requisitos

- .NET SDK 9.0 o superior
- SQL Server (Express, Developer o LocalDB)
- Herramienta de EF Core: `dotnet tool install --global dotnet-ef`

### Pasos

```bash
git clone https://github.com/sidnyzh/TANDA.git
cd TANDA
dotnet restore
dotnet build
```

Configura la cadena de conexión en `src/Tanda.Web/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=1335;Database=Tanda;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

> La conexión usa **autenticación de Windows** (`Trusted_Connection=True`), de modo que no hay credenciales en el repositorio. Reemplaza `1335` por el nombre de tu instancia de SQL Server.

Aplica las migraciones y ejecuta:

```bash
dotnet ef database update --project src/Tanda.Infrastructure --startup-project src/Tanda.Web
dotnet run --project src/Tanda.Web
```

La aplicación queda disponible en `https://localhost:7xxx` (el puerto lo indica la consola).

---

## Base de datos y migraciones

El `DbContext` vive en `Tanda.Infrastructure`, no en el proyecto web, así que la herramienta de EF Core no puede construirlo por sí sola al diseñar. Por eso el proyecto incluye un `IDesignTimeDbContextFactory`, y todos los comandos de migración deben indicar los dos proyectos:

```bash
# crear una migración
dotnet ef migrations add NombreDeLaMigracion \
  --project src/Tanda.Infrastructure \
  --startup-project src/Tanda.Web

# aplicarla
dotnet ef database update \
  --project src/Tanda.Infrastructure \
  --startup-project src/Tanda.Web

# revertir a una migración anterior
dotnet ef database update NombreAnterior \
  --project src/Tanda.Infrastructure \
  --startup-project src/Tanda.Web
```

### Convenciones del modelo

- Montos en `decimal(18,2)` — nunca `float` ni `double`
- Fechas de calendario como `DateOnly`, mapeadas a `date`
- Instantes como `DateTimeOffset`, para no perder el desplazamiento horario
- Índice único filtrado sobre los nombres del catálogo
- Restricciones `CHECK` para los invariantes que la base puede garantizar

---

## Roles y acceso

| Rol | Puede |
|---|---|
| **Administrador** | Gestionar el catálogo, los parámetros del negocio y los usuarios |
| **Vendedor** | Cotizar, confirmar, consultar y cancelar pedidos |
| **Panadero** | Consultar la programación de producción y marcar avances |

Los roles se crean al arrancar la aplicación mediante un *seeder*. El primer usuario administrador se asigna desde ahí; los demás se registran y reciben su rol desde la gestión de usuarios.

---

## Pruebas

```bash
# ejecutar la suite
dotnet test

# ejecutar con cobertura
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover

# generar el informe legible
reportgenerator -reports:**/coverage.opencover.xml -targetdir:coveragereport
```

Las pruebas del dominio no requieren base de datos ni configuración: cada regla recibe valores simples y retorna un resultado. La suite corre en segundos y puede ejecutarse en cada compilación.

El énfasis está en la **cobertura de ramas**, no de líneas. Una condición compuesta puede alcanzar el 100 % de líneas con dos casos y dejar la mitad de las combinaciones sin ejercitar. Objetivo: ≥ 70 % de ramas en la capa de reglas, 100 % en las clases que manejan dinero.

Los valores frontera de cada regla están especificados en el documento de pruebas estructurales (ver [Documentación](#documentación)) y se trasladan al código como casos parametrizados con `[Theory]`, `[InlineData]` y `[MemberData]`.

---

## Calidad de código

- **Analizadores del SDK**: las advertencias en `Tanda.Domain` se tratan como errores de compilación.
- **`.editorconfig`** en la raíz: fija las convenciones de nomenclatura y formato.
- **SonarQube Cloud**: analiza cada rama desde integración continua. Detecta condiciones siempre verdaderas o falsas, código inalcanzable y complejidad ciclomática excesiva — justamente los defectos que el análisis de caminos busca prevenir.

El análisis de C# necesita observar la compilación, así que se ejecuta envuelto en el escáner: iniciar → compilar → probar con cobertura → cerrar y publicar.

---

## Flujo de trabajo

El proyecto se gestiona en Jira con épicas, historias de usuario y tareas, organizadas en sprints.

**Ramas**

```
main        ← versión estable
develop     ← integración
feature/HU-00X-descripcion-corta
```

**Commits** — un commit por tarea, referenciando su clave de Jira:

```
HU-003 Agregar cálculo de precio con adiciones

Implementa OrderPriceCalculator con cobertura de ciclo
para 0, 1 y N adiciones.
```

**Pull requests** — plantilla de tres secciones:

```markdown
## Contexto
Qué problema o necesidad origina el cambio.

## Solución
Cómo se resolvió y por qué se eligió ese camino.

## Cambios realizados
- Archivo o componente afectado y qué cambió en él
```

Antes de integrar a `develop`: las pruebas pasan, la puerta de calidad de SonarQube está en verde y la cobertura de ramas no retrocede.

---

## Estado del desarrollo

| Historia | Descripción | Estado |
|---|---|---|
| HU-001 | Gestión del catálogo de productos, tamaños y adiciones | ✅ Completada |
| HU-002 | Registro y consulta de clientes | 🔜 En curso |
| HU-003 | Cotizar un encargo | ⏳ Planeada |
| HU-004 | Confirmar el pedido mediante el registro del abono | ⏳ Planeada |
| HU-010 | Cancelar un pedido aplicando la política de devolución | ⏳ Planeada |

HU-003, HU-004 y HU-010 son las historias analizadas en el diseño de pruebas estructurales: concentran la lógica de decisión y conforman el flujo completo del encargo —cotización, confirmación y cancelación—.

---

## Documentación

| Entregable | Contenido |
|---|---|
| **RTF1** | Descripción del negocio, actores y proceso actual (BPMN) |
| **RTF2** | Especificación de requisitos: historias de usuario, reglas de negocio, requisitos no funcionales y matriz de trazabilidad |
| **RTF3** | Diseño de pruebas estructurales: diagramas de clases, componentes, actividad y secuencia; análisis de caminos, complejidad ciclomática, valores frontera y herramientas |

---

## Autoría

**Sidny Zapata Hoyos**
Ingeniería de Software II — Facultad de Ingeniería
Institución Universitaria Pascual Bravo · Medellín
Asesor: Juan Camilo Palacio Alcaraz

---

<sub>Proyecto académico. No apto para uso en producción sin una revisión de seguridad y de manejo de datos personales.</sub>
