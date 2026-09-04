# Reto 2: análisis de puntos de dolor

## Alcance y criterio

El análisis usa como AS-IS la solución ubicada en `Trabajo Farmacia/03-Src`. Solo se incluyen rigideces relacionadas con la creación, composición y coordinación de objetos. El costo distingue archivos y tipos existentes que habría que revisar o modificar de los nuevos que habría que crear. Los escenarios de evolución ilustran el costo; no autorizan a cambiar las salidas actuales.

La columna `Origen` indica quién encontró el punto de dolor. No indica quién propuso el patrón o la solución posterior. P-01 se detectó con apoyo de IA; P-02 a P-05 surgieron de la revisión del equipo.

La prioridad es alta cuando el punto afecta SC-3 o exige ampliar varias estructuras por cada variante. Es media cuando hay duplicación comprobada en dos o más archivos, pero el flujo actual funciona. Es baja cuando el cambio está localizado y la solución costaría más que mantenerlo.

| ID | Dónde | Punto de dolor | Costo actual | Prioridad | Decisión | Origen |
|---|---|---|---:|---|---|---|
| P-01 | `Program.cs:273-278`; `ServicioVenta.cs:24-32` | La búsqueda de productos está duplicada entre la consulta y la venta | 2 archivos / 2 clases | Media | Centralizar sin patrón | IA asistida |
| P-02 | `ServicioVenta.cs`; `IDescuento.cs`; `ServicioDescuento.cs`; `Program.cs` | La venta no admite políticas comerciales variables | 4 archivos / 4 tipos | Alta | Intervenir con Strategy | Propio |
| P-03 | `ServicioMonitoreoProductos.cs`; eventos; `Program.cs` | Cada alerta nueva amplía la detección, el evento y la composición | 2 archivos/clases existentes + 1 archivo/clase nuevo por alerta | Alta | Intervenir con Composite | Propio |
| P-04 | Los tres `Cargador*Txt.cs` | El algoritmo de carga TXT está triplicado | 3 archivos / 3 clases | Media | Intervenir con Template Method | Propio |
| P-05 | `Program.cs:203-413` | El menú está centralizado en un `switch` | 1 archivo / 1 clase implícita | Baja | No intervenir | Propio |

## P-01: búsqueda de productos duplicada

**Dónde:** la opción 3 del menú busca directamente en `Program.cs:273-278`. La opción 4 llama a `ServicioVenta.BuscarProducto`, cuya implementación está en `ServicioVenta.cs:24-32`.

**Qué ocurre:** ambos lugares recorren la colección, toman la primera coincidencia parcial del nombre e ignoran diferencias entre mayúsculas y minúsculas. El criterio es el mismo, pero está escrito dos veces.

**Escenario y costo:** si se autorizara priorizar una coincidencia exacta antes de una parcial, habría que cambiar `Program` y `ServicioVenta`: **2 archivos / 2 clases**. Si solo se modifica uno, consultar y vender podrían seleccionar productos distintos con la misma entrada.

**Cambio propuesto:** `ServicioProducto`, que administra el catálogo, incorporará `BuscarPorNombre(string nombre)`. Las opciones 3 y 4 usarán esa operación. `ServicioVenta` dejará de buscar productos y conservará su operación `Vender(Producto, int)`. SC-3 reutilizará la misma búsqueda para la venta con convenio.

La centralización inicial toca `Program`, `ServicioVenta` y `ServicioProducto`: **3 archivos / 3 clases**. Después, cambiar el criterio costará **1 archivo / 1 clase**. No se crean clases.

**Por qué no se adopta un patrón:** solo existe un criterio de búsqueda. Introducir Strategy, Repository u otra abstracción agregaría estructura sin un segundo comportamiento que la justifique. El cambio consiste en asignar la búsqueda al servicio que ya administra el catálogo.

**Origen:** hallazgo asistido por IA y comprobado por el equipo en el código citado.

## P-02: la venta no admite políticas comerciales variables

**Dónde:** `ServicioVenta.cs:35-52` fija la secuencia de la venta normal. `IDescuento.cs:9-12` solo recibe un precio, `ServicioDescuento.cs:11-16` aplica un 10 % fijo y `Program.cs:51-60` no conecta ese servicio con la venta.

**Qué ocurre:** no hay un punto donde elegir una regla comercial según el convenio del cliente. SC-3 solicita convenios para descuentos y crédito descontable con entidades. El diseño adopta las modalidades de descuento, crédito y ambas combinadas. Sin un punto de variación, esas reglas terminarían como condicionales dentro de la venta o del menú.

**Escenario y costo:** para integrar el descuento existente habría que revisar `ServicioVenta`, `IDescuento`, `ServicioDescuento` y `Program`: **4 archivos / 4 tipos (3 clases y 1 interfaz)**. Cada modalidad nueva añadiría otra rama al flujo central.

**Cambio propuesto:** una venta con convenio usará `IPoliticaConvenio` y tres estrategias, una por cálculo: solo descuento, solo crédito y descuento con crédito. La entidad será un dato de `Convenio`, no una estrategia. `SolicitudConvenio` llevará únicamente los valores que necesita la evaluación y `ResultadoConvenio` no modificará estado. `ServicioVentaConvenio` confirmará stock, movimiento y cupo cuando la evaluación sea aprobada. La venta normal conservará su comportamiento.

**Interpretación adoptada para SC-3:** el Anexo B solicita convenios para descuentos y crédito descontable, sin detallar su operación. Para esta entrega se modela el crédito como una compra cuyo total se descuenta del cupo disponible; no como otro descuento sobre el precio. Se adopta cero o un convenio por cliente y tres modalidades: descuento, crédito y descuento con crédito. En la modalidad combinada primero se aplica el descuento y después se consume el cupo por el total resultante. El cupo se mantiene en memoria; no se implementan cobros, cuotas, intereses, descuentos de nómina ni persistencia. Estas son decisiones de alcance del diseño, no reglas detalladas que el enunciado imponga literalmente.

**Origen:** punto encontrado por el equipo al revisar el flujo de venta y compararlo con SC-3.

## P-03: cada alerta nueva amplía varias estructuras

**Dónde:** `ServicioMonitoreoProductos.cs:8-18` mantiene dos eventos concretos; `20-31` verifica stock y `33-47` verifica vencimiento. `Program.cs:62-87` construye el monitor y conecta ambos canales.

**Qué ocurre:** cada regla tiene su propio recorrido y publica mediante un evento concreto. Agregar una alerta obliga a crear el evento, ampliar el monitor y cambiar el ensamblaje.

**Escenario y costo:** para una alerta de sobrestock con evento propio, hoy se modifican `ServicioMonitoreoProductos` y `Program`: **2 archivos / 2 clases existentes**. Se crea además el evento: **1 archivo / 1 clase nueva**.

**Costo después del cambio:** se modifica `Program` (**1 archivo / 1 clase existente**) y se crean la hoja y su evento (**2 archivos / 2 clases nuevas**). Ambos escenarios involucran tres archivos. La mejora es reducir de dos a una las clases existentes modificadas y conservar el algoritmo coordinador y las reglas anteriores.

**Cambio propuesto:** `IReglaAlerta` será el contrato común. `ReglaStockMinimo` y `ReglaVencimiento` serán las hojas, y `MonitorCompuesto` implementará el mismo contrato para ejecutar una colección ordenada de reglas. Los eventos existentes se conservan.

**Comparación con una lista simple:** un coordinador que recorra una lista de reglas también resuelve la extensión de alertas y es una alternativa válida. Se mantiene Composite de forma mínima: las dos hojas y el grupo actual de stock-vencimiento exponen `Verificar(IEnumerable<Producto>)` mediante `IReglaAlerta`. `Program` invoca el grupo a través de ese contrato y configura sus miembros al construirlo. Frente a un coordinador con la misma lista y una API propia, el costo adicional es que ese mismo coordinador implemente la interfaz; no se añaden clases ni agrupaciones hipotéticas. El beneficio específico es una operación uniforme para la regla individual y el conjunto. No se atribuye al patrón exclusividad para ejecutar todas las reglas ni una reducción del total de archivos por alerta.

**Origen:** punto encontrado por el equipo durante la revisión del monitor y su composición.

## P-04: algoritmo de carga TXT triplicado

**Dónde:** `CargadorProductosTxt.cs:22-63`, `CargadorClientesTxt.cs:13-43` y `CargadorUsuariosTxt.cs:13-45` repiten la misma estructura de control.

**Qué ocurre:** los tres validan la existencia del archivo, leen las líneas, separan campos, convierten una fila, agregan el resultado y manejan errores. Solo cambia la conversión de los campos a la entidad correspondiente.

**Escenario y costo:** aceptar encabezados o ignorar comentarios obligaría a modificar los tres cargadores: **3 archivos / 3 clases**.

**Cambio propuesto:** `CargadorTxt<T>` concentrará el flujo común. Cada cargador implementará `ParsearCampos(string[] campos)` y conservará su mensaje de carga. `CargadorClientesTxt` también interpretará los campos opcionales de convenio requeridos por SC-3.

**Origen:** punto encontrado por el equipo al comparar los tres cargadores.

## P-05: menú centralizado en un `switch`

**Dónde:** `Program.cs:203-216` presenta las opciones y `Program.cs:218-413` contiene sus flujos.

**Escenario y costo actual:** agregar una opción como "Ver movimientos" exige modificar **1 archivo / 1 clase implícita**.

**Decisión:** no se interviene. Aplicar Command exigiría al menos una interfaz, siete comandos para las opciones actuales y cambios en `Program`: **9 archivos / 9 tipos (8 clases y 1 interfaz)**. El sistema tiene un solo menú y no necesita historial, deshacer ni otra interfaz que reutilice los comandos. El remedio cuesta más que la rigidez actual.

**Origen:** punto encontrado y descartado por el equipo durante la revisión del menú.

## Conclusión

P-01 se resuelve con una operación común en `ServicioProducto`, sin añadir un patrón. P-02, P-03 y P-04 justifican Strategy, Composite y Template Method. P-05 queda como deuda aceptada porque su solución propuesta sería más costosa que el problema actual.
