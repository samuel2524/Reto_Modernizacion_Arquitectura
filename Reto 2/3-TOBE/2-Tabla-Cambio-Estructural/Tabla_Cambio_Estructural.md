# Tabla de cambio estructural del TO-BE

Esta tabla es la fuente única para los diagramas y la implementación. Separa los cambios por patrón y deja P-01 aparte porque se resuelve sin añadir uno. `Program` aparece una sola vez como cambio transversal, aunque participa en varias conexiones.

## P-01: búsqueda común de productos, sin patrón

| ID | Elemento | Estado | Qué hacía antes | Qué hace ahora | Quién dependía de él y cómo se reconecta |
|---|---|---|---|---|---|
| E-01 | `ServicioProducto` | Se transforma | Administraba la colección y la carga de productos, pero no ofrecía una búsqueda. | Añade `BuscarPorNombre(string nombre)` con el criterio actual: primera coincidencia parcial, sin distinguir mayúsculas. | Las opciones 3 y 4 de `Program` y la ruta de convenio buscan por medio de este servicio. |
| E-02 | `ServicioVenta` | Se transforma | Dependía de `ServicioProducto` para implementar una segunda búsqueda por nombre. | Elimina `BuscarProducto` y la dependencia de `ServicioProducto`. `Vender(Producto, int)` conserva su comportamiento. | `Program` obtiene el producto con `ServicioProducto` y lo entrega a `Vender`. `ServicioMovimiento` sigue conectado igual. |

P-01 cambia dos servicios y la composición de `Program`, pero no crea clases. La búsqueda se mueve; no se modifica su resultado.

## Strategy: P-02 y SC-3

**Interpretación adoptada para SC-3:** el Anexo B solicita convenios para descuentos y crédito descontable, sin detallar su operación. Para esta entrega se modela el crédito como una compra cuyo total se descuenta del cupo disponible; no como otro descuento sobre el precio. Se adopta cero o un convenio por cliente y tres modalidades: descuento, crédito y descuento con crédito. En la modalidad combinada primero se aplica el descuento y después se consume el cupo por el total resultante. El cupo se mantiene en memoria; no se implementan cobros, cuotas, intereses, descuentos de nómina ni persistencia. Estas son decisiones de alcance del diseño, no reglas detalladas que el enunciado imponga literalmente.

| ID | Elemento | Estado | Qué hacía antes | Qué hace ahora | Quién dependía de él y cómo se reconecta |
|---|---|---|---|---|---|
| E-03 | `IDescuento` | Sale | Definía un cálculo que solo recibía un precio. | Se elimina porque no representa un convenio, el crédito ni el resultado completo de la evaluación. | Lo reemplaza `IPoliticaConvenio`. No tenía consumidores conectados a la venta. |
| E-04 | `ServicioDescuento` | Sale | Aplicaba siempre un descuento del 10 % y no participaba en `ServicioVenta`. | Se elimina para evitar dos modelos comerciales sin integración. | No requiere reconexión directa; las nuevas políticas cubren los cálculos de SC-3. |
| E-05 | `Cliente` | Se transforma | Guardaba identificación, nombre, teléfono, correo y puntos. | Incorpora cero o un `Convenio`. | `CargadorClientesTxt` lo construye y `ServicioVentaConvenio` consulta su convenio y confirma el cupo. |
| E-06 | `Convenio` | Entra | No existía. | Guarda la entidad como texto, el tipo de beneficio, el porcentaje y el cupo disponible. | Pertenece opcionalmente a `Cliente`. Sus valores alimentan `SolicitudConvenio`. |
| E-07 | `TipoBeneficioConvenio` | Entra | No existía. | Enumera descuento, crédito y descuento con crédito. Se amplía al incorporar una modalidad nueva. | `ServicioVentaConvenio` lo usa como clave para seleccionar la estrategia. |
| E-08 | `SolicitudConvenio` | Entra | No existía. | Contiene los datos mínimos e inmutables para evaluar: subtotal, porcentaje y cupo disponible. | La construye `ServicioVentaConvenio`. No transporta `Cliente`, `Producto`, cantidad ni servicios. |
| E-09 | `ResultadoConvenio` | Entra | No existía. | Informa aprobación, descuento, total, cupo restante calculado y motivo opcional. No modifica estado. | Lo crea una política y lo interpreta `ServicioVentaConvenio`. |
| E-10 | `IPoliticaConvenio` | Entra | No había un punto de variación comercial dentro del caso de uso. | Declara `Evaluar(SolicitudConvenio) : ResultadoConvenio`. | `ServicioVentaConvenio` depende de este contrato, no de una política concreta. |
| E-11 | `PoliticaSoloDescuento` | Entra | No existía. | Calcula el descuento y deja intacto el cupo recibido. | Implementa `IPoliticaConvenio` y no modifica objetos externos. |
| E-12 | `PoliticaSoloCredito` | Entra | No existía. | Compara el subtotal con el cupo y calcula el cupo restante cuando autoriza. | Implementa `IPoliticaConvenio` y no modifica objetos externos. |
| E-13 | `PoliticaDescuentoCredito` | Entra | No existía. | Aplica el descuento y evalúa el total resultante contra el cupo. | Implementa `IPoliticaConvenio` y no modifica objetos externos. |
| E-14 | `ServicioVentaConvenio` | Entra | Solo existía la venta normal. | Selecciona la política, prepara la solicitud y evalúa. Si se aprueba, confirma stock, movimiento y cupo; si se rechaza, no cambia esos datos. | Recibe el registro de estrategias y `ServicioMovimiento`. Opera con el cliente y producto que resuelve `Program`. |
| E-15 | `clientes.txt` | Se transforma | Cada fila tenía cuatro campos para los datos básicos del cliente. | Admite esos cuatro campos y, de forma opcional, entidad, beneficio, porcentaje y cupo. | `CargadorClientesTxt` sigue leyendo el archivo y permite clientes con o sin convenio. |

No entra `TipoEntidadConvenio`. `Convenio.Entidad` guarda el nombre o identificación de la entidad concreta; no solo una categoría como banco o universidad. La categoría no determina la política.

`ServicioVenta.Vender` conserva el flujo normal. Su única transformación pertenece a P-01: deja de buscar el producto.

## Composite: P-03

| ID | Elemento | Estado | Qué hacía antes | Qué hace ahora | Quién dependía de él y cómo se reconecta |
|---|---|---|---|---|---|
| E-16 | `ServicioMonitoreoProductos` | Sale | Contenía directamente las verificaciones de stock y vencimiento y conocía los dos eventos. | Se elimina; sus reglas pasan a hojas independientes y la coordinación pasa al compuesto. | `Program` deja de construirlo y usa `MonitorCompuesto`. |
| E-17 | `IReglaAlerta` | Entra | No existía un contrato común para reglas y grupos de reglas. | Declara `Verificar(IEnumerable<Producto> productos) : void`. | Lo implementan las dos hojas y `MonitorCompuesto`. |
| E-18 | `ReglaStockMinimo` | Entra | La lógica estaba en `ServicioMonitoreoProductos.VerificarStock`. | Hoja que conserva el criterio, el evento y el texto de stock mínimo. | `Program` conecta su evento y la agrega primero al compuesto. |
| E-19 | `ReglaVencimiento` | Entra | La lógica estaba en `ServicioMonitoreoProductos.VerificarVencimiento`. | Hoja que conserva el criterio, el evento y el texto de vencimiento. | `Program` conecta su evento y la agrega después de la regla de stock. |
| E-20 | `MonitorCompuesto` | Entra | No existía. | Implementa `IReglaAlerta`, contiene una colección ordenada de `IReglaAlerta` y ejecuta todas. | `Program` construye las dos hojas y el grupo; lo invoca como `IReglaAlerta` para verificar productos. |

Se conservan `EventoStockMinimo`, `EventoVencimiento`, sus mensajes, colores y el orden stock-vencimiento. Cada hoja recorre todos los productos antes de pasar a la siguiente.

**Comparación con una lista simple:** un coordinador que recorra una lista de reglas también resuelve la extensión de alertas y es una alternativa válida. Se mantiene Composite de forma mínima: las dos hojas y el grupo actual de stock-vencimiento exponen `Verificar(IEnumerable<Producto>)` mediante `IReglaAlerta`. `Program` invoca el grupo a través de ese contrato y configura sus miembros al construirlo. Frente a un coordinador con la misma lista y una API propia, el costo adicional es que ese mismo coordinador implemente la interfaz; no se añaden clases ni agrupaciones hipotéticas. El beneficio específico es una operación uniforme para la regla individual y el conjunto. No se atribuye al patrón exclusividad para ejecutar todas las reglas ni una reducción del total de archivos por alerta.

Para una alerta nueva con evento propio: antes, dos archivos existentes modificados y uno nuevo; después, `Program` modificado y dos archivos nuevos (hoja y evento). No se modifican el compuesto ni las reglas existentes.

## Template Method: P-04

| ID | Elemento | Estado | Qué hacía antes | Qué hace ahora | Quién dependía de él y cómo se reconecta |
|---|---|---|---|---|---|
| E-21 | `CargadorTxt<T>` | Entra | El algoritmo común estaba repetido en tres cargadores. | Controla validación, lectura, recorrido, `Split`, parseo delegado, agregado, errores y mensaje final. | Es la base de los tres cargadores concretos. `Program` no la conoce directamente. |
| E-22 | `CargadorProductosTxt` | Se transforma | Ejecutaba el algoritmo completo y hacía el parseo de productos. | Hereda de `CargadorTxt<Producto>`, implementa `ParsearCampos` y conserva el selector de creadores. | Mantiene `ICargadorProductos`; `ServicioProducto` sigue usándolo por esa interfaz. |
| E-23 | `CargadorClientesTxt` | Se transforma | Ejecutaba el algoritmo completo y leía los cuatro primeros campos. | Hereda de `CargadorTxt<Cliente>`, implementa `ParsearCampos` y crea el convenio cuando estén presentes los campos opcionales. | Mantiene `ICargadorClientes`; `ServicioCliente` sigue usándolo por esa interfaz. |
| E-24 | `CargadorUsuariosTxt` | Se transforma | Ejecutaba el algoritmo completo y hacía el parseo de usuarios. | Hereda de `CargadorTxt<Usuario>` e implementa `ParsearCampos`. | Mantiene `ICargadorUsuarios`; `ServicioUsuario` sigue usándolo por esa interfaz. |

Cada cargador conserva su mensaje: `Productos cargados`, `Clientes cargados` o `Usuarios cargados`.

## Cambio transversal

| ID | Elemento | Estado | Qué hacía antes | Qué hace ahora | Quién dependía de él y cómo se reconecta |
|---|---|---|---|---|---|
| E-25 | `Program` | Se transforma | Buscaba directamente en la opción 3, delegaba otra búsqueda a `ServicioVenta`, construía el monitor anterior y no exponía SC-3. | Usa `ServicioProducto.BuscarPorNombre` en consulta y venta, registra las estrategias, construye el Composite y reconoce `--convenio`. | Sin el argumento conserva el menú y las salidas existentes. Con `--convenio` coordina la nueva venta sin duplicar la búsqueda. |

## Resumen

| Grupo | Sale | Entra | Se transforma |
|---|---:|---:|---:|
| P-01 sin patrón | 0 | 0 | 2 |
| Strategy / SC-3 | 2 | 9 | 2 |
| Composite | 1 | 4 | 0 |
| Template Method | 0 | 1 | 3 |
| Cambio transversal | 0 | 0 | 1 |
| **Total** | **3** | **14** | **8** |

El resumen cuenta clases, interfaces, el enum y el archivo `clientes.txt`. `CargadorClientesTxt` se cuenta una sola vez en Template Method, aunque también se adapta para SC-3. `Program` se cuenta una sola vez como cambio transversal.

## Elementos conservados

- `ServicioVenta.Vender` y el flujo de la venta normal.
- `ICargadorProductos`, `ICargadorClientes` e `ICargadorUsuarios`.
- `EventoStockMinimo` y `EventoVencimiento`.
- Los creadores y el selector de productos.
- El mecanismo Observer existente.
- Factory Method como parte del AS-IS.
