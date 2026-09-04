# Análisis de patrones del Reto 2

## Alcance

El diseño conserva el comportamiento de la venta normal y las salidas cubiertas por los 12 casos de caracterización. La solicitud elegida es SC-3: convenios para descuentos y crédito descontable. Cada cliente podrá tener cero o un convenio, y el cupo se mantendrá en memoria. No se incorporan cuotas, persistencia, red ni un framework de inyección.

**Interpretación adoptada para SC-3:** el Anexo B solicita convenios para descuentos y crédito descontable, sin detallar su operación. Para esta entrega se modela el crédito como una compra cuyo total se descuenta del cupo disponible; no como otro descuento sobre el precio. Se adopta cero o un convenio por cliente y tres modalidades: descuento, crédito y descuento con crédito. En la modalidad combinada primero se aplica el descuento y después se consume el cupo por el total resultante. El cupo se mantiene en memoria; no se implementan cobros, cuotas, intereses, descuentos de nómina ni persistencia. Estas son decisiones de alcance del diseño, no reglas detalladas que el enunciado imponga literalmente.

`Convenio.Entidad` guardará el nombre o identificación de la entidad concreta, sea empresa, banco, cooperativa, universidad o colegio. Las estrategias cambian por el cálculo del beneficio, no por el tipo de entidad.

## Tabla de decisión

| Patrón evaluado | Familia | Punto que podría atender | Qué gana y qué cuesta | Decisión | Razón |
|---|---|---|---|---|---|
| Factory Method | Creacional | P-01 | No elimina la búsqueda duplicada. Introducir otra fábrica desviaría el problema hacia la creación de objetos. | Descartado | El patrón ya existe para crear productos y se conserva como parte del AS-IS, pero no atiende P-01. |
| Builder | Creacional | P-02 / SC-3 | Podría construir `Convenio`, pero sumaría un constructor y pasos para un objeto con pocos datos. | Descartado | El cambio requerido está en el cálculo comercial, no en una construcción compleja. |
| Abstract Factory | Creacional | P-02 / SC-3 | Agruparía familias de objetos de convenio, pero exigiría fábricas e interfaces para una familia que no existe. | Descartado | Las variantes son comportamientos de cálculo. |
| Composite | Estructural | P-03 | Permite tratar reglas y grupos con el mismo contrato. Cuesta una interfaz, dos hojas y un compuesto. | **Adoptado** | Todas las alertas deben ejecutarse y el monitor no debe crecer por cada regla. |
| Facade | Estructural | P-02 | Daría una entrada única al flujo de convenio, pero añadiría otra capa de coordinación. | Descartado | `ServicioVentaConvenio` ya coordina el caso de uso; otra fachada no reduciría dependencias reales. |
| Strategy | Comportamiento | P-02 | Separa los tres cálculos y permite seleccionarlos en ejecución. Cuesta una interfaz, tres estrategias y registro en `Program`. | **Adoptado** | El alcance adoptado para SC-3 distingue tres cálculos; su selección queda separada de la venta. |
| Template Method | Comportamiento | P-04 | Reúne el algoritmo TXT. Cuesta una clase base y herencia en tres cargadores. | **Adoptado** | Los pasos son iguales y solo cambia el parseo de campos. |
| Chain of Responsibility | Comportamiento | P-03 | Puede ejecutar todos los manejadores si cada uno continúa; añade enlaces o una política de paso al siguiente. | Descartado | No se necesita que una regla decida la continuidad. El grupo ordenado controla explícitamente la ejecución de todas. |
| Command | Comportamiento | P-05 | Separaría las opciones del menú, pero exigiría una interfaz y al menos siete comandos. | Descartado | Hoy el cambio cuesta un solo archivo y no hay historial, deshacer ni otro cliente del menú. |

Se evaluaron tres patrones creacionales, dos estructurales y cuatro de comportamiento. Los únicos patrones nuevos del TO-BE son Strategy, Composite y Template Method.

## P-01 sin patrón: búsqueda común de productos

`Program.cs:273-278` y `ServicioVenta.cs:24-32` repiten la misma búsqueda parcial por nombre. El costo actual de cambiar el criterio es de **2 archivos / 2 clases**.

`ServicioProducto` incorporará `BuscarPorNombre(string nombre)` y conservará el criterio actual: primera coincidencia parcial, sin distinguir mayúsculas. Las opciones 3 y 4 del menú usarán esa operación. `ServicioVenta` dejará de buscar y seguirá recibiendo el producto en `Vender`.

La primera modificación cuesta **3 archivos / 3 clases**: `Program`, `ServicioVenta` y `ServicioProducto`. Después, el criterio se cambiará en un único archivo. No se aplica Strategy porque solo existe una forma de buscar. Tampoco se crea un repositorio: `ServicioProducto` ya administra la colección.

## Strategy para P-02 y SC-3

`IDescuento.cs:9-12` solo calcula a partir de un precio y `ServicioDescuento.cs:11-16` fija un 10 %. Ninguno participa en `ServicioVenta.cs:35-52`. Estos dos elementos salen porque no representan crédito, convenio ni el resultado completo de una evaluación comercial.

El modelo de apoyo queda formado por:

- `Convenio`: entidad como texto, tipo de beneficio, porcentaje y cupo disponible.
- `TipoBeneficioConvenio`: descuento, crédito o descuento con crédito.
- `SolicitudConvenio`: subtotal, porcentaje y cupo disponible. No contiene referencias mutables a `Cliente`, `Producto` o `Convenio`.
- `ResultadoConvenio`: aprobación, descuento, total, cupo restante calculado y motivo opcional. Representa una evaluación; no confirma una venta.

Los participantes del patrón son `IPoliticaConvenio`, `PoliticaSoloDescuento`, `PoliticaSoloCredito`, `PoliticaDescuentoCredito` y `ServicioVentaConvenio` como contexto. Las políticas implementan `Evaluar(SolicitudConvenio)` y no modifican estado.

El flujo será el siguiente:

1. `Program` obtiene el cliente y el producto mediante sus servicios.
2. `ServicioVentaConvenio` valida que el cliente tenga convenio y calcula el subtotal una sola vez.
3. El servicio selecciona la política mediante `TipoBeneficioConvenio`.
4. La política evalúa y devuelve `ResultadoConvenio` sin efectos laterales.
5. Si el resultado rechaza la operación, no cambian stock, movimientos ni cupo.
6. Si la aprueba, `ServicioVentaConvenio` descuenta stock, registra el movimiento y aplica el cupo restante.

`Program` registra las tres implementaciones y las entrega al servicio. Agregar una modalidad con cálculo nuevo y los mismos datos de entrada exige crear su estrategia, ampliar `TipoBeneficioConvenio` y registrarla en `Program`: un archivo nuevo y dos existentes modificados, sin cambiar la venta normal. Si necesita datos adicionales, también habrá que revisar el contrato y la carga; ese costo no queda resuelto automáticamente por Strategy. No se crea `TipoEntidadConvenio` porque ninguna regla cambia por ser banco, empresa, cooperativa, universidad o colegio.

## Composite para P-03

`ServicioMonitoreoProductos.cs:20-47` contiene dos recorridos y conoce eventos concretos. Una alerta nueva obliga a tocar ese servicio, crear el evento y cambiar `Program`.

`IReglaAlerta` definirá la operación común. `ReglaStockMinimo` y `ReglaVencimiento` serán hojas. `MonitorCompuesto` también implementará `IReglaAlerta`, contendrá una colección ordenada de reglas y las ejecutará todas. `ServicioMonitoreoProductos` sale.

Los eventos `EventoStockMinimo` y `EventoVencimiento` se conservan. `Program` conecta sus suscriptores y agrega primero la regla de stock y después la de vencimiento para mantener orden, texto y color. Una alerta nueva con evento propio requiere su hoja y su evento (dos archivos nuevos), y modificar `Program` (un archivo existente). No cambia el compuesto ni las reglas anteriores. Antes se modificaban dos archivos existentes y se creaba uno: el total sigue siendo tres; disminuye la intervención sobre código existente.

**Comparación con una lista simple:** un coordinador que recorra una lista de reglas también resuelve la extensión de alertas y es una alternativa válida. Se mantiene Composite de forma mínima: las dos hojas y el grupo actual de stock-vencimiento exponen `Verificar(IEnumerable<Producto>)` mediante `IReglaAlerta`. `Program` invoca el grupo a través de ese contrato y configura sus miembros al construirlo. Frente a un coordinador con la misma lista y una API propia, el costo adicional es que ese mismo coordinador implemente la interfaz; no se añaden clases ni agrupaciones hipotéticas. El beneficio específico es una operación uniforme para la regla individual y el conjunto. No se atribuye al patrón exclusividad para ejecutar todas las reglas ni una reducción del total de archivos por alerta.

## Template Method para P-04

`CargadorProductosTxt.cs:22-63`, `CargadorClientesTxt.cs:13-43` y `CargadorUsuariosTxt.cs:13-45` repiten validación, lectura, recorrido, separación, agregado, mensajes y manejo de errores.

`CargadorTxt<T>` tendrá el algoritmo común. El método plantilla ejecutará `Split(';')` y delegará la conversión en `ParsearCampos(string[] campos)`. Cada cargador también proporcionará su mensaje de carga para conservar las salidas actuales.

Las interfaces `ICargadorProductos`, `ICargadorClientes` e `ICargadorUsuarios` se mantienen. `CargadorClientesTxt` interactúa con el diseño de SC-3 porque construirá el `Convenio` opcional cuando la fila tenga sus campos. Las filas actuales de cuatro campos seguirán creando clientes sin convenio.

Una función estática genérica que reciba el parser y el mensaje también podría centralizar todo el algoritmo. Se prefiere Template Method para expresar los pasos especializados como miembros obligatorios de los tres cargadores existentes y conservar su construcción. La alternativa estática es viable; la herencia se acepta como costo, no como la única forma de eliminar la duplicación.

El costo es una clase base y cambios en tres cargadores. La plantilla solo tendrá pasos que los tres puedan cumplir; no se agregarán métodos vacíos para resolver diferencias particulares.

## Colaboración del TO-BE

```text
Program / Composition Root
|-- ServicioProducto.BuscarPorNombre
|-- ServicioVenta normal
|-- ServicioVentaConvenio
|   `-- IPoliticaConvenio (Strategy)
|       |-- PoliticaSoloDescuento
|       |-- PoliticaSoloCredito
|       `-- PoliticaDescuentoCredito
|-- MonitorCompuesto : IReglaAlerta (Composite)
|   |-- ReglaStockMinimo
|   `-- ReglaVencimiento
`-- CargadorTxt<T> (Template Method)
    |-- CargadorProductosTxt
    |-- CargadorClientesTxt
    `-- CargadorUsuariosTxt
```

## Decisión final

El TO-BE incorpora Strategy, Composite y Template Method. P-01 se resuelve con una operación común en `ServicioProducto`. Factory Method y Observer permanecen como parte del AS-IS y no se cuentan como patrones incorporados en este reto.
