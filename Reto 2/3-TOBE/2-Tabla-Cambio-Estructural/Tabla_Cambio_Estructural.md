# Tabla de cambio estructural - TO-BE Reto 2

Registro de cada elemento que cambia al incorporar Strategy, Composite, Template Method y la consolidacion de Factory Method. Para cada elemento se indica que estado tenia, que pasa a hacer, y quien dependia de el y como se reconecta. El contenido es consistente con las fichas de patron (Actividad 3.3) y con el analisis de patrones.

## Estrategia (Strategy) - responde a P-02 / SC-3

| ID | Elemento | Estado | Que hacia antes | Que hace ahora | Quien dependia de el y como se reconecta |
|---|---|---|---|---|---|
| E-01 | `ServicioVenta.Vender` | Se transforma | `ServicioVenta.cs:35-52`: descuenta inventario, crea el movimiento y lo registra, sin punto de variacion comercial. | Conserva exactamente ese flujo para la venta normal. El criterio comercial se atiende en una ruta nueva (`ServicioVentaConvenio`) que no pasa por aqui. | `Program` y los 12 casos de caracterizacion dependian de `Vender`; siguen dependiendo igual y la salida no cambia. La venta con convenio es una ruta aparte. |
| E-02 | `IPoliticaConvenio` | Entra | No existia. | Contrato que expone `Evaluar(SolicitudConvenio)`: subtotal, descuento, total, autorizacion de credito, cupo restante y motivo de rechazo. | Nadie dependia. `ServicioVentaConvenio` se conecta a el como estrategia. |
| E-03 | `PoliticaSoloDescuento` / `PoliticaSoloCredito` / `PoliticaDescuentoCredito` | Entran | No existian. | Tres estrategias concretas; cada una implementa `IPoliticaConvenio` con su propio calculo. | `Program` las registra en el conjunto de estrategias. `ServicioVentaConvenio` las selecciona. |
| E-04 | `ServicioVentaConvenio` | Entra | No existia. | Atiende la venta con convenio: recibe la politica, la invoca y devuelve el resultado. | `Program` lo construye y lo expone como nueva opcion. No modifica `ServicioVenta`. |

## Compuesto (Composite) - responde a P-03

| ID | Elemento | Estado | Que hacia antes | Que hace ahora | Quien dependia de el y como se reconecta |
|---|---|---|---|---|---|
| E-05 | `ServicioMonitoreoProductos` | Se transforma | `ServicioMonitoreoProductos.cs:20-31` y `33-48`: verificaba stock y vencimiento con dos metodos propios y dos eventos concretos. | Deja de contener las reglas; pasa a delegar la verificacion en las hojas del compuesto. | `Program.cs` (antiguo ensamblaje en 62-87) sigue construyendo el monitoreo, ahora armando el `MonitorCompuesto` con sus reglas. |
| E-06 | `IReglaAlerta` | Entra | No existia. | Contrato comun de una regla de alerta. | `ReglaStockMinimo` y `ReglaVencimiento` la implementan; `MonitorCompuesto` la usa. |
| E-07 | `ReglaStockMinimo` / `ReglaVencimiento` | Entran | No existian como clases (la logica vivia en los metodos del monitor). | Hojas que encapsulan cada regla y publican su alerta. | `MonitorCompuesto` las compone y las ejecuta; el `Observer` existente conserva la publicacion. |
| E-08 | `MonitorCompuesto` | Entra | No existia. | Compone las reglas y las ejecuta todas. | `Program` lo arma. `ServicioMonitoreoProductos` (o su sustituto) lo invoca al verificar. |

## Plantilla (Template Method) - responde a P-04

| ID | Elemento | Estado | Que hacia antes | Que hace ahora | Quien dependia de el y como se reconecta |
|---|---|---|---|---|---|
| E-09 | `CargadorProductosTxt` | Se transforma | `CargadorProductosTxt.cs:22-63`: repetia todo el flujo de carga y hacia su propio parseo. | Hereda de `CargadorTxt<T>` y conserva solo `ParsearLinea`, que convierte una fila en `Producto`. | `ICargadorProductos` y `Program` no cambian; el cargador concreto se construye igual. |
| E-10 | `CargadorClientesTxt` | Se transforma | `CargadorClientesTxt.cs:13-43`: mismo flujo repetido (validar, leer, recorrer, split, agregar, error). | Hereda de `CargadorTxt<T>` y conserva solo `ParsearLinea` para `Cliente`. | `ICargadorClientes` y `Program` no cambian. |
| E-11 | `CargadorUsuariosTxt` | Se transforma | `CargadorUsuariosTxt.cs:13-45`: mismo flujo repetido. | Hereda de `CargadorTxt<T>` y conserva solo `ParsearLinea` para `Usuario`. | `ICargadorUsuarios` y `Program` no cambian. |
| E-12 | `CargadorTxt<T>` | Entra | No existia. | Clase base con el algoritmo comun de carga TXT y el paso abstracto `ParsearLinea`. | Los tres cargadores heredan de ella; `Program` no la conoce directamente. |

## Consolidacion de Factory Method - responde a P-01

| ID | Elemento | Estado | Que hacia antes | Que hace ahora | Quien dependia de el y como se reconecta |
|---|---|---|---|---|---|
| E-13 | `ICreadorProducto`, `CreadorMedicamentoCapsula`, `SelectorCreadorProducto` | Se conservan | Ya eran el mecanismo unico de creacion desde el Reto 1. | Siguen igual: cada creador convierte un `DatosProducto` en producto. | `CargadorProductosTxt` y `Program` siguen dependiendo igual; no se rompe ningun consumidor. |

## Resumen

| Cambio | Sale | Entra | Se transforma |
|---|---|---|---|
| Strategy | 0 | 4 (E-02, E-03, E-04) | 1 (E-01) |
| Composite | 0 | 3 (E-06, E-07, E-08) | 1 (E-05) |
| Template Method | 0 | 1 (E-12) | 3 (E-09, E-10, E-11) |
| Factory Method | 0 | 0 | 0 (se conserva) |

Ningun elemento se elimina: las interfaces formales (`ICargadorProductos`, `ICargadorClientes`, `ICargadorUsuarios`, `IDescuento`, `ServicioDescuento`) y el `Observer` existente se conservan para no cambiar el contrato de los consumidores ni la salida del programa.
