# Analisis de patrones para SC-3

## Alcance

La venta normal debe conservar su flujo y su salida para que los 12 casos existentes sigan funcionando. SC-3 se agregara como una opcion aparte para ventas con convenio. Cada cliente podra tener cero o un convenio, con descuento, credito o ambos. El cupo se mantendra en memoria y no se manejaran cuotas ni persistencia.

Empresa, banco, cooperativa, universidad y colegio se modelaran como datos de la entidad. No tiene sentido crear una clase distinta para cada uno si sus reglas comerciales son iguales.

## Patrones evaluados

| Patron | Familia | Punto | Decision | Justificacion |
|---|---|---|---|---|
| Factory Method | Creacional | P-01 | Mantener y consolidar | Sera el unico mecanismo de creacion; ya existe en el AS-IS. |
| Abstract Factory | Creacional | P-01/P-02 | Descartar | No hay familias de objetos que justifiquen varias fabricas relacionadas. |
| Facade | Estructural | P-02 | Descartar | Strategy cubre la variacion sin agregar otra clase coordinadora. |
| Composite | Estructural | P-03 | **Adoptar** | Permite reunir las alertas sin ampliar el monitor por cada regla nueva. |
| Strategy | Comportamiento | P-02 | **Adoptar** | Permite cambiar la politica del convenio sin llenar la venta de condicionales. |
| Template Method | Comportamiento | P-04 | **Adoptar** | Reune en un solo lugar el algoritmo comun de carga TXT. |
| Chain of Responsibility | Comportamiento | P-03 | Descartar | Las alertas deben ejecutarse todas, no detenerse en el primer manejador. |
| Command | Comportamiento | P-05 | Descartar | Agregaria nueve clases para un cambio que hoy se hace en un archivo. |

## Strategy para SC-3

El contrato actual no alcanza para manejar convenios. `IDescuento.cs:9-12` solo recibe un precio y `ServicioDescuento.cs:13-16` aplica siempre el 10 %. Ademas, `ServicioVenta.cs:35-52` no conoce al cliente, la entidad ni el cupo disponible.

La nueva opcion usara `ServicioVentaConvenio`, que elegira una implementacion de `IPoliticaConvenio`. El metodo `Evaluar(SolicitudConvenio)` devolvera el subtotal, el descuento, el total, la autorizacion del credito, el cupo restante y el motivo de rechazo cuando corresponda.

Se contemplan tres politicas: `PoliticaSoloDescuento`, `PoliticaSoloCredito` y `PoliticaDescuentoCredito`. Estas clases existen porque calculan cosas distintas, no por el nombre o el tipo de entidad.

Agregar una politica requerira una implementacion y su registro en `Program`. La venta normal no cambia. El costo inicial es una interfaz, tres estrategias y modificaciones en `ServicioVentaConvenio` y `Program`.

Strategy favorece OCP y DIP porque la venta depende del contrato `IPoliticaConvenio`. Ese contrato debe limitarse a la evaluacion comercial para no terminar con una interfaz demasiado amplia.

## Factory Method en P-01

Hoy hay dos caminos para crear medicamentos. `CreadorMedicamentoCapsula.cs:10-26` usa los datos leidos del archivo, mientras `ProductoFactory.cs:13-42` crea capsulas y liquidos con valores definidos dentro de la fabrica. La carga actual usa el primer camino y `ProductoFactory` no tiene consumidores.

El TO-BE conservara `ICreadorProducto`, los creadores concretos y `SelectorCreadorProducto`. `ProductoFactory` se eliminara para que no queden dos politicas de construccion para el mismo tipo de producto.

El cambio elimina un archivo y una clase, no agrega abstracciones y no modifica la salida del programa. Factory Method ya estaba en el AS-IS, por lo que se documenta como una consolidacion y no como un patron nuevo del Reto 2.

## Composite para las alertas

`ServicioMonitoreoProductos.cs:20-47` recorre los productos por separado para revisar stock y vencimiento. Con la estructura actual, una alerta nueva obliga a tocar el monitor, crear su evento y cambiar la composicion.

La propuesta usa `IReglaAlerta` como contrato comun. `ReglaStockMinimo` y `ReglaVencimiento` seran las hojas, mientras que `MonitorCompuesto` las agrupara y ejecutara. Observer se mantiene para publicar los mensajes.

Al principio hay que crear la interfaz, separar las dos reglas y recomponer el monitor. Despues, una alerta nueva solo necesita su regla y el registro correspondiente. Esto mejora SRP y OCP siempre que todas las reglas puedan ejecutarse con el mismo contrato.

## Template Method para los TXT

Los cargadores de productos, clientes y usuarios repiten el mismo proceso. Esto se ve en `CargadorProductosTxt.cs:22-63`, `CargadorClientesTxt.cs:13-43` y `CargadorUsuariosTxt.cs:13-45`: validar el archivo, leerlo, recorrer las lineas, separar campos, agregar el resultado y manejar errores.

`CargadorTxt<T>` tendra ese flujo comun y cada cargador implementara `ParsearLinea(string linea)`. Asi, una regla general del formato se cambia una vez y no en tres archivos.

El costo es una clase base y cambios en los tres cargadores. Los pasos de la plantilla deben ser comunes para todos; agregar metodos vacios u opcionales pondria en riesgo LSP.

## Por que se descartan los otros patrones

- Facade no reduce las construcciones de `Program`. Tambien agregaria dos clases para delegar una seleccion que `ServicioVentaConvenio` puede hacer con Strategy.
- Abstract Factory no tiene una familia real de objetos que crear. Agregar una interfaz y varias fabricas solo esconderia construcciones que hoy son simples.
- Chain of Responsibility no representa bien las alertas porque stock y vencimiento deben revisarse siempre. Composite expresa mejor esa relacion.
- Command llevaria cada opcion del menu a una clase distinta, aunque no hay historial, deshacer ni otra interfaz que reutilice esos comandos.

## Como quedaria la colaboracion

```text
Program / Composition Root
|-- Venta normal sin cambios
|-- Nueva venta con convenio
|   `-- ServicioVentaConvenio
|       `-- IPoliticaConvenio (Strategy)
|-- MonitorCompuesto (Composite)
|   |-- ReglaStockMinimo
|   `-- ReglaVencimiento
`-- CargadorTxt<T> (Template Method)
    |-- CargadorProductosTxt
    |-- CargadorClientesTxt
    `-- CargadorUsuariosTxt
```

## Decision final

Los patrones nuevos seran Strategy, Composite y Template Method. Factory Method se consolidara y Observer seguira como parte del AS-IS, sin contarlos como incorporaciones del Reto 2. Facade y Command quedan descartados porque agregan mas estructura de la que el proyecto necesita.
