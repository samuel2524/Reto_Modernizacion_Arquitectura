# Ficha de patrón: Strategy

## Patrón y punto de dolor que resuelve

P-02: `ServicioVenta.cs:35-52` registra ventas sin evaluar convenios; `IDescuento.cs:9-12` recibe solo un precio y `ServicioDescuento.cs:11-16` fija el 10 %. SC-3 solicita descuentos y crédito descontable. El equipo interpreta crédito como consumo de cupo en memoria y adopta descuento, crédito o ambos; no incorpora cuotas ni cobros.

## Alternativas evaluadas

**No hacer nada:** descartado; las modalidades quedarían como condicionales en venta o menú. **Facade:** descartada; otra entrada no separa los cálculos que ya coordina el servicio. **Abstract Factory:** descartada; no hay familias de objetos relacionados. **Strategy:** adoptada para seleccionar cálculos mediante un contrato.

## Qué sale y qué entra

Salen `IDescuento` y `ServicioDescuento`, sin consumidores en la venta. Entran `IPoliticaConvenio`, las tres políticas y `ServicioVentaConvenio` como contexto. Los tipos de apoyo son `Convenio`, `TipoBeneficioConvenio`, `SolicitudConvenio` y `ResultadoConvenio`. No entra `TipoEntidadConvenio`.

## Cómo se relaciona

`Program` registra las políticas por beneficio y construye el servicio con `ServicioMovimiento`. El servicio calcula el subtotal y entrega valores inmutables: subtotal, porcentaje y cupo. `Evaluar(SolicitudConvenio)` devuelve aprobación, descuento, total, cupo calculado y motivo. Las políticas no modifican estado. Un rechazo no cambia stock, movimientos ni cupo; la aprobación permite al servicio confirmarlos. Descuento conserva el cupo; crédito consume el subtotal; la combinación consume el total descontado. Template Method carga el convenio opcional; P-01 aporta la búsqueda común.

## Impacto

Entran siete clases, una interfaz y un enum. Cambian `Cliente`, `CargadorClientesTxt`, `Program` y `clientes.txt`; salen una clase y una interfaz. Se permiten cero o un convenio por cliente y filas antiguas de cuatro campos. SC-3 usa una ruta propia; no se implementa SC-2 ni se altera SC-1 o la venta normal.

## Qué cuesta

Nueve tipos y más pasos para depurar. Una modalidad nueva con el mismo contrato exige una estrategia nueva, ampliar el enum y modificar el registro en `Program`. Datos nuevos exigirían revisar también contrato y carga. Se acepta este costo para separar los tres cálculos adoptados.

## Origen

Según los registros del equipo: B-03 acepta Strategy; B-04 distingue políticas por cálculo, no por entidad; B-05 rechaza otra Facade. La ampliación del costo y el alcance explícito proceden de esta revisión asistida y deben reflejarse en la bitácora en preparación.
