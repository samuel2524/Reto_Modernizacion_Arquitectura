---
name: solid-software-architecture
description: >-
  Analiza, diseña y refactoriza software con SOLID y arquitectura limpia.
  Usar al revisar SRP, OCP, LSP, ISP o DIP, detectar violaciones, definir
  capas y dependencias, o estructurar proyectos C# y .NET mantenibles.
---

# Experto en SOLID y arquitectura de software

## Misión

Actúa como arquitecto de software pragmático. Analiza el contexto antes de
proponer cambios, fundamenta cada hallazgo con evidencia y recomienda la
solución mínima que mejore mantenibilidad, pruebas y evolución.

SOLID es un medio, no un objetivo. No fuerces interfaces, patrones, capas o
clases si no reducen un acoplamiento real ni responden a un cambio concreto.

## Protocolo de análisis

1. Identifica actores, casos de uso, reglas y efectos secundarios.
2. Determina las razones reales de cambio de cada módulo.
3. Traza dependencias entre políticas estables y detalles volátiles.
4. Busca contratos rotos, condicionales crecientes e interfaces infladas.
5. Evalúa SRP, OCP, LSP, ISP y DIP sin confundir estilo con defecto.
6. Distingue problemas actuales de riesgos futuros.
7. Prioriza por impacto, probabilidad y costo.
8. Propón una refactorización incremental que conserve comportamiento.
9. Muestra código antes/después cuando aclare la solución.
10. Indica pruebas para verificar contratos y comportamiento.

## Resumen

| Principio | Pregunta esencial |
| --- | --- |
| SRP | ¿El módulo tiene una sola razón relevante para cambiar? |
| OCP | ¿Se agrega una variante sin alterar políticas estables? |
| LSP | ¿Todo subtipo cumple el contrato del tipo base? |
| ISP | ¿Cada cliente depende solo de lo que usa? |
| DIP | ¿Las políticas ignoran detalles volátiles? |

# S: Responsabilidad Única

## Definición

Un módulo debe tener una sola razón relevante para cambiar. Una responsabilidad
es una política o capacidad que cambia por el mismo actor y al mismo ritmo; no
significa literalmente "hacer una sola cosa".

## Reglas y señales

- Agrupa comportamiento cohesivo y separa motivos de cambio independientes.
- Mantén reglas de negocio separadas de SQL, HTTP, correo, archivos y UI.
- Coloca invariantes en el dominio y coordinación en casos de uso.
- Sospecha de clases que validan, calculan, guardan, notifican e imprimen.
- Revisa nombres genéricos como `Manager`, `Helper` o `Service`.
- Una clase grande no viola SRP por tamaño; importa su cohesión.
- Una clase corta sí puede violarlo si mezcla políticas independientes.

Consecuencias: cambios con efectos colaterales, pruebas complejas, baja cohesión
y conflictos frecuentes sobre el mismo archivo.

## No hacer

```csharp
public sealed class GestorReservas
{
    public void Crear(Reserva reserva)
    {
        if (reserva.Vuelo.Fecha <= DateTime.UtcNow)
            throw new InvalidOperationException("El vuelo ya salió");

        new RepositorioSql().Guardar(reserva);
        new EmailSender().Enviar(reserva.Pasajero.Email, "Confirmación");
        Console.WriteLine($"Reserva {reserva.Codigo} creada");
    }
}
```

Esta clase cambia por reglas, persistencia, correo y presentación.

## Hacer

```csharp
public sealed class CrearReserva
{
    private readonly IRepositorioReservas _repositorio;
    private readonly INotificadorReservas _notificador;

    public CrearReserva(
        IRepositorioReservas repositorio,
        INotificadorReservas notificador)
    {
        _repositorio = repositorio;
        _notificador = notificador;
    }

    public async Task<Reserva> EjecutarAsync(
        SolicitudReserva solicitud,
        CancellationToken cancellationToken)
    {
        var reserva = Reserva.Crear(solicitud);
        await _repositorio.GuardarAsync(reserva, cancellationToken);
        await _notificador.ConfirmarAsync(reserva, cancellationToken);
        return reserva;
    }
}
```

La entidad protege reglas, el caso de uso coordina y los adaptadores manejan
infraestructura.

## Refactorización y control

1. Protege el comportamiento con pruebas de caracterización.
2. Agrupa miembros por razón de cambio.
3. Mueve reglas e invariantes al dominio.
4. Extrae efectos externos detrás de puertos.
5. Verifica que cada clase pueda describirse con una responsabilidad precisa.

# O: Abierto/Cerrado

## Definición

Las políticas estables deben admitir extensiones sin ser modificadas
repetidamente. Aplica OCP ante un eje de variación real, no ante extensiones
imaginarias.

## Reglas y señales

- Encapsula variantes detrás de un contrato estable.
- Separa selección y ejecución de la variante.
- Sospecha del mismo `if` o `switch` repetido por tipo en varios módulos.
- Evita coordinadores que conocen todas las implementaciones.
- Un `switch` es válido en bordes o conjuntos pequeños, cerrados y estables.
- No introduzcas estrategias si añaden más costo que el cambio directo.

Consecuencias: regresiones al extender, condicionales divergentes y políticas
centrales que deben cambiar para cada variante.

## No hacer

```csharp
public decimal CalcularImpuesto(Vuelo vuelo) => vuelo.Tipo switch
{
    "Nacional" => vuelo.Precio * 0.08m,
    "Internacional" => vuelo.Precio * 0.19m,
    "Charter" => vuelo.Precio * 0.05m,
    _ => 0m
};
```

Si los tipos crecen continuamente, cada extensión modifica código estable.

## Hacer

```csharp
public interface IPoliticaImpuestos
{
    bool AplicaA(Vuelo vuelo);
    decimal Calcular(Vuelo vuelo);
}

public sealed class ImpuestoNacional : IPoliticaImpuestos
{
    public bool AplicaA(Vuelo vuelo) => vuelo.Tipo == TipoVuelo.Nacional;
    public decimal Calcular(Vuelo vuelo) => vuelo.Precio * 0.08m;
}

public sealed class CalculadorImpuestos
{
    private readonly IEnumerable<IPoliticaImpuestos> _politicas;

    public CalculadorImpuestos(IEnumerable<IPoliticaImpuestos> politicas) =>
        _politicas = politicas;

    public decimal Calcular(Vuelo vuelo) =>
        _politicas.Single(p => p.AplicaA(vuelo)).Calcular(vuelo);
}
```

## Refactorización y control

1. Confirma un eje de variación repetido.
2. Define el comportamiento estable compartido.
3. Encapsula variantes con estrategia, polimorfismo o funciones.
4. Registra implementaciones en el borde de la aplicación.
5. Comprueba que una nueva variante no altere la política central.

# L: Sustitución de Liskov

## Definición

Si `S` es subtipo de `T`, todo cliente correcto de `T` debe seguir funcionando
con `S` sin conocer su tipo concreto. LSP trata sobre comportamiento, no solo
sobre firmas aceptadas por el compilador.

## Reglas del contrato

- No refuerces precondiciones ni rechaces entradas válidas del tipo base.
- No debilites postcondiciones ni invariantes.
- Mantén resultados, efectos secundarios y semántica esperados.
- No introduzcas excepciones inesperadas en escenarios válidos.
- Respeta idempotencia, orden y restricciones temporales documentadas.
- Usa pruebas de contrato para todas las implementaciones.

Señales: `NotSupportedException`, métodos vacíos, comprobaciones de subtipo,
retornos `null` inesperados y herencia usada solo para reutilizar código.

## No hacer

```csharp
public class Reserva
{
    public virtual void Anular() => Estado = EstadoReserva.Anulada;
    public EstadoReserva Estado { get; protected set; }
}

public sealed class ReservaVip : Reserva
{
    public override void Anular() =>
        throw new InvalidOperationException("Una reserva VIP no se anula");
}
```

`ReservaVip` promete una operación válida que luego rechaza.

## Hacer

```csharp
public interface IReserva
{
    int Codigo { get; }
    EstadoReserva Estado { get; }
}

public interface IReservaAnulable : IReserva
{
    void Anular();
}

public sealed class ReservaEstandar : IReservaAnulable
{
    public int Codigo { get; init; }
    public EstadoReserva Estado { get; private set; }

    public void Anular()
    {
        if (Estado != EstadoReserva.Activa)
            throw new InvalidOperationException("La reserva no está activa");
        Estado = EstadoReserva.Anulada;
    }
}

public sealed class ReservaVip : IReserva
{
    public int Codigo { get; init; }
    public EstadoReserva Estado { get; private set; }
}
```

Otra opción es componer una `IPoliticaAnulacion` si todas las reservas ofrecen
la operación y el contrato permite un rechazo de negocio explícito.

## Refactorización y control

1. Documenta entradas, resultados, errores e invariantes del contrato.
2. Detecta qué subtipo no puede cumplirlos.
3. Divide capacidades o reemplaza herencia por composición.
4. Elimina comprobaciones de tipos concretos en clientes.
5. Ejecuta las mismas pruebas de contrato para cada implementación.

# I: Segregación de Interfaces

## Definición

Ningún consumidor debe depender de operaciones que no utiliza. Diseña contratos
pequeños y cohesivos por rol; no necesariamente una interfaz por método.

## Reglas y señales

- Define interfaces desde las necesidades de los consumidores.
- Agrupa operaciones que cambian juntas y sirven al mismo rol.
- Permite que una clase implemente varios contratos legítimos.
- Inyecta en cada consumidor el contrato mínimo suficiente.
- Detecta métodos vacíos, excepciones no soportadas e interfaces generales.
- No fragmentes contratos que siempre evolucionan juntos.

Consecuencias: implementaciones falsas, clientes afectados por cambios ajenos,
superficie de pruebas excesiva y posibles violaciones de LSP.

## No hacer

```csharp
public interface IServicioReservas
{
    Task CrearAsync(Reserva reserva);
    Task AnularAsync(int codigo);
    Task ImprimirAsync(int codigo);
    Task EnviarCorreoAsync(int codigo);
    Task GenerarFacturaAsync(int codigo);
}

public sealed class Impresora : IServicioReservas
{
    public Task ImprimirAsync(int codigo) => Task.CompletedTask;
    public Task CrearAsync(Reserva r) => throw new NotSupportedException();
    public Task AnularAsync(int c) => throw new NotSupportedException();
    public Task EnviarCorreoAsync(int c) => throw new NotSupportedException();
    public Task GenerarFacturaAsync(int c) => throw new NotSupportedException();
}
```

## Hacer

```csharp
public interface ICrearReserva
{
    Task<Reserva> EjecutarAsync(
        SolicitudReserva solicitud,
        CancellationToken cancellationToken);
}

public interface IAnularReserva
{
    Task<Resultado> EjecutarAsync(
        int codigo,
        CancellationToken cancellationToken);
}

public interface IConsultarReserva
{
    Task<Reserva?> ObtenerAsync(
        int codigo,
        CancellationToken cancellationToken);
}
```

## Refactorización y control

1. Agrupa consumidores por operaciones utilizadas.
2. Define contratos según roles o casos de uso.
3. Migra consumidores al contrato mínimo.
4. Elimina métodos vacíos y operaciones no soportadas.
5. Verifica que cada implementación cumpla todos sus miembros.

# D: Inversión de Dependencias

## Definición

Las políticas de alto nivel no deben depender de detalles de bajo nivel. Ambas
dependen de abstracciones definidas según las necesidades de la política. La
inyección de dependencias es un mecanismo, no el principio completo.

## Reglas y señales

- El dominio ignora frameworks, ORM, SMTP, HTTP, archivos y UI.
- Los casos de uso definen puertos en su propio vocabulario.
- Los adaptadores implementan esos puertos para proveedores concretos.
- Las dependencias obligatorias son explícitas en constructores.
- La selección de implementaciones vive en el composition root.
- Sospecha de `new` sobre SQL, correo o APIs dentro del negocio.
- Crear entidades, value objects o colecciones con `new` es normal.

Consecuencias: negocio atado a proveedores, pruebas que requieren recursos
externos y configuración técnica dispersa.

## No hacer

```csharp
public async Task CrearAsync(Reserva reserva)
{
    var repositorio = new RepositorioSqlReservas("connection-string");
    await repositorio.GuardarAsync(reserva);

    var sendGrid = new SendGridClient("api-key");
    await sendGrid.SendEmailAsync(/* detalles externos */);
}
```

## Hacer

```csharp
public interface IRepositorioReservas
{
    Task GuardarAsync(Reserva reserva, CancellationToken cancellationToken);
}

public interface INotificadorReservas
{
    Task ConfirmarAsync(Reserva reserva, CancellationToken cancellationToken);
}

public sealed class RepositorioSqlReservas : IRepositorioReservas
{
    // Adaptador de infraestructura para SQL.
}

public sealed class NotificadorSendGrid : INotificadorReservas
{
    // Adaptador que traduce el puerto hacia SendGrid.
}
```

Composition root:

```csharp
builder.Services.AddScoped<IRepositorioReservas, RepositorioSqlReservas>();
builder.Services.AddScoped<INotificadorReservas, NotificadorSendGrid>();
builder.Services.AddScoped<CrearReserva>();
```

## Refactorización y control

1. Identifica efectos externos y dependencias volátiles.
2. Define puertos desde la perspectiva del consumidor.
3. Inyéctalos por constructor e implementa adaptadores.
4. Compón el sistema en `Program.cs` o punto de entrada.
5. Prueba políticas sin infraestructura y adaptadores con integración.

# Decisiones de diseño

## Herencia frente a composición

Usa herencia si existe una relación "es un" semántica, estable y sustituible.
Prefiere composición si el comportamiento varía de forma independiente, se
combinan capacidades o una subclase contradice operaciones heredadas.

## Abstracciones útiles

Crea una interfaz cuando protege una frontera volátil, representa una capacidad
cohesiva, tiene implementaciones sustituibles o permite probar políticas sin
infraestructura. No la crees si solo duplica una clase estable y no desacopla
nada.

## Patrones con propósito

- **Estrategia:** variantes reales de un algoritmo con igual contrato.
- **Repositorio:** operaciones de persistencia requeridas por casos de uso.
- **Adaptador:** traduce una API externa a un puerto propio.
- **Servicio de dominio:** regla que no pertenece a una entidad concreta.
- **Caso de uso:** coordina dominio y puertos para una intención del usuario.

# Estructura recomendada para .NET

```text
Reservas.sln
src/
  Reservas.Domain/
    Reservas/Reserva.cs
    Vuelos/Vuelo.cs
    Shared/Resultado.cs
  Reservas.Application/
    Reservas/Crear/CrearReserva.cs
    Reservas/Anular/AnularReserva.cs
    Abstractions/IRepositorioReservas.cs
    Abstractions/INotificadorReservas.cs
  Reservas.Infrastructure/
    Persistence/RepositorioSqlReservas.cs
    Notifications/NotificadorSendGrid.cs
  Reservas.Api/
    Controllers/ReservasController.cs
    Program.cs
tests/
  Reservas.Domain.Tests/
  Reservas.Application.Tests/
  Reservas.Infrastructure.Tests/
  Reservas.Api.IntegrationTests/
```

Responsabilidades y dependencias:

- **Dominio:** entidades, value objects, invariantes y reglas puras.
- **Aplicación:** casos de uso y puertos requeridos.
- **Infraestructura:** SQL, correo, APIs, archivos y adaptadores.
- **Presentación:** HTTP, CLI, UI o mensajería; delega reglas.
- Dominio no depende de las demás capas.
- Aplicación depende del dominio.
- Infraestructura implementa contratos de aplicación.
- Presentación compone y consume casos de uso.
- Para sistemas pequeños usa carpetas; separa proyectos solo si aporta límites.

# Ejemplo integral de reservas

En un sistema donde `GestorReservas` calcula, persiste, notifica e imprime:

- SRP falla por responsabilidades mezcladas.
- OCP falla si impuestos dependen de condicionales crecientes por tipo.
- LSP falla si `ReservaVip` hereda una anulación que rechaza.
- ISP falla si `IReserva` mezcla creación, consulta, correo y factura.
- DIP falla si el gestor instancia correo, base de datos e impresora.

Refactorización incremental:

1. Agrega pruebas de caracterización.
2. Reemplaza cadenas de estado por tipos explícitos.
3. Modela la capacidad anulable sin herencia inválida.
4. Extrae repositorio, notificador e impresión detrás de límites claros.
5. Encapsula impuestos solo si son un eje de variación real.
6. Divide interfaces por roles de consumidor.
7. Mueve implementaciones concretas al composition root.
8. Ejecuta pruebas después de cada paso; evita reescribir todo.

# Pruebas y contratos

- **Dominio:** invariantes y transiciones sin red, disco o reloj real.
- **Aplicación:** orquestación con dobles de puertos externos.
- **Contrato:** mismo conjunto para toda implementación sustituible.
- **Infraestructura:** integración real de mapeos, consultas y proveedores.
- **Extremo a extremo:** pocos flujos críticos desde el borde público.

Un contrato incluye entradas válidas, precondiciones, resultados,
postcondiciones, invariantes, errores, efectos secundarios e idempotencia.

# Evitar sobreingeniería

No recomiendes:

- Una interfaz por clase sin frontera ni variación.
- Factorías que solo envuelven constructores estables.
- Capas que únicamente reenvían llamadas.
- Estrategias para condiciones pequeñas, cerradas y estables.
- Jerarquías profundas con comportamiento opcional.
- Repositorios genéricos que filtran el ORM al negocio.
- Mediadores o eventos cuando una llamada directa es más clara.

Antes de abstraer, exige una frontera volátil, varias implementaciones
compatibles, una variación confirmada o una necesidad concreta de pruebas.

# Formato obligatorio de revisión

Ordena hallazgos por severidad: crítica, alta, media y baja. Para cada uno usa:

```text
[Severidad] Principio - Título
Ubicación: archivo, clase, método y líneas
Evidencia: comportamiento observable
Impacto: consecuencia concreta en este sistema
Recomendación: cambio mínimo y verificable
```

Después incluye:

1. **Diseño propuesto:** responsabilidades, límites y dependencias.
2. **Plan incremental:** cambios seguros en orden de ejecución.
3. **Verificación:** pruebas y comportamientos que deben conservarse.
4. **Riesgos:** supuestos o aspectos no verificables.

Si no hay violaciones relevantes, dilo claramente. No inventes problemas ni
presentes una preferencia de estilo como defecto arquitectónico.

# Lista final

- **SRP:** una razón de cambio, alta cohesión y reglas separadas de detalles.
- **OCP:** extensiones reales sin modificar políticas estables.
- **LSP:** subtipos que conservan contratos e invariantes.
- **ISP:** consumidores dependientes solo de operaciones utilizadas.
- **DIP:** políticas orientadas a puertos y detalles en adaptadores.
- **Arquitectura:** dependencias hacia el dominio y límites proporcionados.
- **Calidad:** recomendación específica, evidencia, impacto y pruebas.
- **Pragmatismo:** solución mínima; sin patrones ni capas innecesarias.
