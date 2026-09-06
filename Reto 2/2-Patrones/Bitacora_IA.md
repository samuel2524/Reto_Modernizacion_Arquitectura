# Bitácora de decisiones frente a la IA

La bitácora registra decisiones de diseño, no una lista de consultas. Cada entrada indica si el equipo aceptó, corrigió o rechazó la propuesta después de compararla con el código y el alcance del reto.

| ID | Qué consultamos | Qué propuso la IA | Qué hicimos | Argumento del equipo y evidencia |
|---|---|---|---|---|
| B-01 | Qué solicitud de cambio convenía implementar | Trabajar con SC-3 porque introduce distintas reglas comerciales | Aceptamos | SC-3 se relaciona con P-02. `IDescuento.cs:9-12` solo recibe un precio y no permite evaluar al cliente, su convenio ni el crédito. |
| B-02 | Dónde había una rigidez concreta que no repitiera un hallazgo del Reto 1 | Centralizar la búsqueda de productos que está duplicada en la consulta y la venta, sin añadir un patrón | Aceptamos | `Program.cs:273-278` y `ServicioVenta.cs:24-32` aplican el mismo criterio. P-01 quedó como hallazgo asistido por IA. `ServicioProducto` tendrá la búsqueda común; cambiar el criterio pasará de 2 archivos / 2 clases a 1 archivo / 1 clase. |
| B-03 | Cómo manejar las modalidades de convenio | Aplicar Strategy y crear un contrato para la evaluación comercial | Aceptamos | La política debe elegirse durante la venta sin agregar ramas por modalidad al flujo principal. `ServicioVentaConvenio` dependerá de `IPoliticaConvenio`; la venta normal no cambia. |
| B-04 | Si debíamos crear una estrategia para cada entidad | Crear estrategias para empresas, bancos, cooperativas, universidades y colegios | Corregimos | La entidad será un dato de `Convenio`. Las estrategias solo cambian cuando cambia el cálculo: descuento, crédito o ambos. Así se evitan cinco clases con comportamiento repetido. |
| B-05 | Si Strategy necesitaba una Facade adicional | Crear `ServicioConvenios` para coordinar las estrategias | Rechazamos | `ServicioVentaConvenio` ya coordina el caso de uso. La Facade añadiría otra clase sin ocultar un subsistema complejo ni reducir dependencias reales. |
| B-06 | Si Facade podía resolver la concentración de `Program` | Mover el ensamblaje detrás de una fachada | Rechazamos | La Facade solo ocultaría las construcciones y suscripciones de `Program.cs:8-110`. `Program` debe seguir siendo el Composition Root. |
| B-07 | Qué patrón servía para agregar alertas | Usar Chain of Responsibility | Corregimos | Stock y vencimiento deben comprobarse siempre. Composite permite tratar las hojas y el grupo mediante `IReglaAlerta` y ejecutar la colección completa. |
| B-08 | Cómo eliminar la repetición de los cargadores TXT | Aplicar Template Method | Aceptamos | `CargadorProductosTxt.cs:22-63`, `CargadorClientesTxt.cs:13-43` y `CargadorUsuariosTxt.cs:13-45` repiten el flujo. `CargadorTxt<T>` lo concentrará y cada cargador implementará `ParsearCampos`. |
| B-09 | Si convenía separar cada opción del menú en un comando | Aplicar Command | Rechazamos | P-05 cuesta hoy 1 archivo / 1 clase implícita. Command exigiría una interfaz, siete comandos y cambios en `Program`, sin historial, deshacer ni otro cliente del menú. |
| B-10 | Si los convenios justificaban Abstract Factory | Crear una fábrica por familia de convenios | Rechazamos | No hay familias de objetos que deban crearse juntas. La variación está en el cálculo comercial, por lo que Abstract Factory agregaría fábricas e interfaces sin resolver P-02. |
| B-11 | Si el Observer actual debía contarse como patrón nuevo | Sumarlo a Strategy, Composite y Template Method | Corregimos | Observer ya existe en el AS-IS y seguirá publicando las alertas. No es una incorporación del Reto 2 y no se suma al total de patrones adoptados. |
| B-12 | Qué datos necesitaban las políticas y quién debía modificar el estado | Incluir cliente, producto, cantidad y una clasificación de entidad en la solicitud; permitir que la evaluación manejara el resultado completo | Corregimos | `SolicitudConvenio` solo llevará subtotal, porcentaje y cupo. `ResultadoConvenio` informará aprobación, descuento, total, cupo calculado y motivo, sin modificar objetos. `ServicioVentaConvenio` confirmará stock, movimiento y cupo. Tampoco se crea `TipoEntidadConvenio` porque ninguna política usa esa clasificación. |

## Balance de decisiones

| Resultado | Cantidad | Registros |
|---|---:|---|
| Aceptamos | 4 | B-01, B-02, B-03, B-08 |
| Corregimos | 4 | B-04, B-07, B-11, B-12 |
| Rechazamos | 4 | B-05, B-06, B-09, B-10 |
| **Total** | **12** | B-01 a B-12 |
