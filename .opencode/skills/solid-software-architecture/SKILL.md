---
name: solid-software-architecture
description: Analiza, diseña y refactoriza software con SOLID y arquitectura limpia. Usar al revisar SRP, OCP, LSP, ISP o DIP, detectar violaciones, definir capas y dependencias, o estructurar proyectos C# y .NET mantenibles.
---

# SOLID y arquitectura de software

## Misión

Actúa como especialista pragmático en diseño de software. Analiza el contexto
antes de proponer cambios, fundamenta cada hallazgo con evidencia y recomienda
la solución mínima que mejore mantenibilidad, pruebas y evolución.

SOLID es un medio, no un objetivo. No fuerces interfaces, patrones, capas o
clases si no reducen un acoplamiento real ni responden a un cambio concreto.

## Protocolo de análisis

1. Identifica actores, casos de uso, reglas, invariantes y efectos secundarios.
2. Determina las razones reales de cambio de cada módulo.
3. Traza dependencias entre políticas estables y detalles volátiles.
4. Busca contratos rotos, condicionales crecientes e interfaces infladas.
5. Evalúa SRP, OCP, LSP, ISP y DIP sin confundir estilo con defecto.
6. Distingue problemas actuales de riesgos futuros.
7. Prioriza por impacto, probabilidad, costo y reversibilidad.
8. Propón una refactorización incremental que conserve el comportamiento.
9. Muestra código antes y después solamente cuando aclare la solución.
10. Indica pruebas para verificar contratos y comportamiento.

## Preguntas esenciales

| Principio | Pregunta |
| --- | --- |
| SRP | ¿El módulo tiene una sola razón relevante para cambiar? |
| OCP | ¿Una variante real puede agregarse sin alterar políticas estables? |
| LSP | ¿Todo subtipo conserva el contrato del tipo base? |
| ISP | ¿Cada consumidor depende únicamente de las operaciones que utiliza? |
| DIP | ¿Las políticas de negocio ignoran los detalles volátiles? |

# SRP: Responsabilidad Única

## Criterio

Un módulo debe tener una sola razón relevante para cambiar. Una responsabilidad
es una política o capacidad que cambia por el mismo actor y al mismo ritmo; no
significa literalmente que una clase solo pueda realizar una operación.

## Señales

- Una clase valida, calcula, persiste, notifica e imprime.
- Las reglas de negocio están mezcladas con SQL, HTTP, archivos o UI.
- Actores distintos solicitan cambios frecuentes en el mismo módulo.
- Las pruebas requieren preparar muchas dependencias no relacionadas.
- Un cambio localizado produce efectos colaterales en otras capacidades.

El tamaño por sí solo no demuestra una violación. Una clase grande puede ser
cohesiva y una clase pequeña puede mezclar políticas independientes.

## Refactorización

1. Protege el comportamiento con pruebas de caracterización.
2. Agrupa miembros por razón de cambio.
3. Coloca invariantes en el dominio.
4. Coloca coordinación en casos de uso.
5. Extrae efectos externos detrás de límites cuando reduzca acoplamiento real.
6. Verifica que cada módulo pueda describirse con una responsabilidad precisa.

## Ejemplo

Evita que un caso de uso cree directamente una conexión SQL, envíe correo y
escriba en consola. El caso de uso debe coordinar la operación y delegar los
efectos externos a colaboradores explícitos.

# OCP: Abierto/Cerrado

## Criterio

Las políticas estables deben admitir extensiones ante ejes de variación
confirmados sin modificarse repetidamente. No diseñes extensibilidad para
escenarios imaginarios.

## Señales

- El mismo `if` o `switch` por tipo aparece en varios módulos.
- Cada nueva variante obliga a modificar una política central estable.
- Un coordinador conoce todas las implementaciones concretas.
- Las ramas para variantes divergen y dejan reglas sin aplicar.

Un `switch` es válido en un borde de composición o cuando el conjunto es
pequeño, cerrado y estable.

## Refactorización

1. Confirma que la variación existe y se repite.
2. Define el comportamiento estable compartido.
3. Encapsula variantes mediante estrategia, polimorfismo o funciones.
4. Separa la selección de la ejecución.
5. Registra implementaciones en el borde de la aplicación.
6. Comprueba que agregar una variante no altere la política central.

## Ejemplo C#

```csharp
public interface IPoliticaDescuento
{
    bool AplicaA(Venta venta);
    decimal Calcular(Venta venta);
}

public sealed class CalculadorDescuentos
{
    private readonly IEnumerable<IPoliticaDescuento> _politicas;

    public CalculadorDescuentos(IEnumerable<IPoliticaDescuento> politicas) =>
        _politicas = politicas;

    public decimal Calcular(Venta venta) =>
        _politicas.Where(p => p.AplicaA(venta)).Sum(p => p.Calcular(venta));
}
```

Usa esta estructura únicamente si existen varias políticas reales que cambian
de forma independiente. Para una regla única y estable, una función directa es
más simple.

# LSP: Sustitución de Liskov

## Criterio

Si `S` es subtipo de `T`, todo cliente correcto de `T` debe continuar
funcionando con `S` sin conocer el tipo concreto. LSP trata sobre comportamiento
y contratos, no solamente sobre firmas aceptadas por el compilador.

## Reglas del contrato

- No refuerces precondiciones.
- No rechaces entradas válidas para el tipo base.
- No debilites postcondiciones ni invariantes.
- Conserva resultados, errores y efectos secundarios esperados.
- No agregues excepciones inesperadas en escenarios válidos.
- Respeta semántica, idempotencia y restricciones documentadas.

## Señales

- Métodos que lanzan `NotSupportedException` en un subtipo.
- Implementaciones vacías para satisfacer una clase base o interfaz.
- Retornos `null` inesperados.
- Comprobaciones frecuentes del tipo concreto.
- Herencia usada únicamente para reutilizar código.

## Refactorización

1. Documenta entradas, resultados, errores e invariantes del contrato.
2. Detecta qué subtipo no puede cumplirlos.
3. Divide capacidades o reemplaza herencia por composición.
4. Elimina comprobaciones de tipos concretos en los consumidores.
5. Ejecuta las mismas pruebas de contrato para cada implementación.

## Ejemplo

Si no todos los productos pueden vencerse, no declares una operación de
vencimiento obligatoria que algunos subtipos deban rechazar. Modela una
capacidad `IProductoConVencimiento` o compón una política de vencimiento.

# ISP: Segregación de Interfaces

## Criterio

Ningún consumidor debe depender de operaciones que no utiliza. Diseña contratos
pequeños y cohesivos por rol o caso de uso, no necesariamente una interfaz por
método.

## Señales

- Implementaciones con métodos vacíos.
- Operaciones que lanzan `NotSupportedException`.
- Interfaces generales para consumidores con necesidades distintas.
- Cambios en una operación que obligan a recompilar clientes no relacionados.
- Dobles de prueba que deben implementar demasiados miembros irrelevantes.

## Refactorización

1. Agrupa consumidores por las operaciones que realmente usan.
2. Define contratos desde las necesidades de cada rol.
3. Mantén juntas las operaciones que evolucionan juntas.
4. Permite que una clase implemente varios contratos legítimos.
5. Elimina miembros no soportados y verifica todas las implementaciones.

## Ejemplo C#

```csharp
public interface IConsultarProductos
{
    Producto? ObtenerPorId(int id);
}

public interface IActualizarInventario
{
    void Descontar(int productoId, int cantidad);
}
```

No dividas una interfaz si todos sus consumidores necesitan las mismas
operaciones y estas cambian juntas.

# DIP: Inversión de Dependencias

## Criterio

Las políticas de alto nivel no deben depender de detalles volátiles. Los casos
de uso definen los puertos que necesitan y la infraestructura implementa los
adaptadores. La inyección de dependencias es un mecanismo, no el objetivo.

## Señales

- El negocio utiliza directamente archivos, SQL, SMTP, HTTP o `Console`.
- Un caso de uso instancia clientes de servicios externos.
- Las pruebas necesitan disco, red o reloj real.
- La configuración técnica está dispersa por la lógica de negocio.
- Sustituir un proveedor obliga a modificar políticas centrales.

Crear entidades, value objects y colecciones con `new` es normal. No abstraigas
tipos estables solamente para eliminar todas las construcciones directas.

## Refactorización

1. Identifica efectos externos y dependencias volátiles.
2. Define puertos desde el vocabulario del consumidor.
3. Inyecta las dependencias obligatorias por constructor.
4. Implementa adaptadores en infraestructura.
5. Selecciona implementaciones en el composition root.
6. Prueba políticas sin infraestructura y adaptadores mediante integración.

## Ejemplo C#

```csharp
public interface IRepositorioProductos
{
    Producto? ObtenerPorId(int id);
    void Guardar(Producto producto);
}

public interface IReloj
{
    DateTime Ahora { get; }
}

public sealed class RegistrarVenta
{
    private readonly IRepositorioProductos _productos;

    public RegistrarVenta(IRepositorioProductos productos) =>
        _productos = productos;
}
```

La interfaz debe expresar lo que necesita el caso de uso, no copiar toda la API
de una base de datos o proveedor.

# Diseño de límites

Cuando el tamaño y riesgo del sistema lo justifiquen, organiza:

- **Dominio:** entidades, value objects, invariantes y reglas puras.
- **Aplicación:** casos de uso y puertos requeridos.
- **Infraestructura:** archivos, SQL, correo, APIs y adaptadores.
- **Presentación:** HTTP, CLI, UI o mensajería; delega reglas.
- **Composition root:** configuración de implementaciones concretas.

Las dependencias deben apuntar hacia las políticas estables. En aplicaciones
pequeñas, usa carpetas o módulos; separa proyectos solamente si el límite aporta
aislamiento verificable.

# Pruebas y contratos

- Dominio: invariantes y transiciones sin red, disco o reloj real.
- Aplicación: coordinación mediante dobles de puertos externos.
- Contrato: el mismo conjunto para cada implementación sustituible.
- Infraestructura: integración real de consultas, mapeos y proveedores.
- Extremo a extremo: pocos flujos críticos desde el borde público.

Un contrato incluye entradas válidas, precondiciones, resultados,
postcondiciones, invariantes, errores, efectos secundarios e idempotencia.

# Evitar sobreingeniería

No recomiendes sin evidencia:

- Una interfaz por clase.
- Factorías que solo envuelven constructores estables.
- Capas que únicamente reenvían llamadas.
- Estrategias para condiciones pequeñas, cerradas y estables.
- Jerarquías profundas con comportamiento opcional.
- Repositorios genéricos que filtran detalles del ORM.
- Mediadores o eventos cuando una llamada directa es más clara.
- Microservicios para resolver modularidad interna.

Antes de abstraer, exige una frontera volátil, variaciones confirmadas,
implementaciones sustituibles, aislamiento útil o una necesidad concreta de
pruebas.

# Formato de revisión

Ordena los hallazgos por severidad: crítica, alta, media y baja. Para cada uno:

```text
[Severidad] Principio - Título
Ubicación: archivo, clase, método y líneas
Evidencia: comportamiento observable o dependencia concreta
Impacto: consecuencia específica en este sistema
Recomendación: cambio mínimo y verificable
```

Después incluye, solamente si aporta valor:

1. Diseño propuesto: responsabilidades, límites y dependencias.
2. Plan incremental: cambios seguros en orden de ejecución.
3. Verificación: pruebas y comportamientos que deben conservarse.
4. Riesgos: supuestos o aspectos no verificables.

Si no hay violaciones relevantes, indícalo claramente. No inventes problemas
ni presentes preferencias de estilo como defectos arquitectónicos.

# Lista final

- **SRP:** una razón de cambio, alta cohesión y reglas separadas de detalles.
- **OCP:** extensiones reales sin modificar políticas estables.
- **LSP:** subtipos que conservan contratos e invariantes.
- **ISP:** consumidores dependientes solo de operaciones utilizadas.
- **DIP:** políticas orientadas a puertos y detalles en adaptadores.
- **Arquitectura:** dependencias hacia el dominio y límites proporcionados.
- **Calidad:** recomendaciones específicas con evidencia, impacto y pruebas.
- **Pragmatismo:** solución mínima sin patrones ni capas innecesarias.
