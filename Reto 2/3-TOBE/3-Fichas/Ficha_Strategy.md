# Ficha de patron - Strategy

**Patron y punto de dolor que resuelve**

`Strategy` responde a **P-02** (la venta no admite politicas comerciales variables). La evidencia esta en `ServicioVenta.cs:35-52`: `Vender` descuenta inventario, crea el movimiento y lo registra sin ningun punto donde elegir una regla comercial antes de completar la operacion. El contrato de `IDescuento.cs:9-12` solo recibe un precio y `ServicioDescuento.cs:11-16` aplica siempre el 10 %. SC-3 exige seleccionar descuentos y credito segun empresa, banco, cooperativa o institucion; hoy esa variacion tendria que hundirse en condicionales dentro de `ServicioVenta`.

**Alternativas que evaluamos**

1. **No hacer nada** (descartada): dejaria la variacion comercial como condicionales dentro de `ServicioVenta`, que es exactamente el punto rígido que P-02 declara; cada convenio nuevo agregaria ramas al flujo central de la venta.
2. **Facade** (descartada): `ServicioConvenios` coordinaría las estrategias, pero `Program` ya es el Composition Root y no hay un subsistema complejo que ocultar; agregaria dos clases sin resolver la seleccion.
3. **Abstract Factory** (descartada): no hay una familia real de objetos que crear de forma coordinada; solo cambia el cálculo de la politica.
4. **Strategy** (adoptada): la venta con convenio elige una implementacion de `IPoliticaConvenio` en tiempo de ejecucion.

**Que sale y que entra**

*Sale:* la idea de que `ServicioVenta` fije internamente el criterio comercial. `IDescuento` y `ServicioDescuento` se conservan sin cambios (la venta normal no los usa hoy y su conducta no se altera).

*Entra:* `IPoliticaConvenio` (contrato) con su metodo `Evaluar(SolicitudConvenio)`, que devuelve subtotal, descuento, total, autorizacion del credito, cupo restante y motivo de rechazo. Entran tres implementaciones estrategicas: `PoliticaSoloDescuento`, `PoliticaSoloCredito` y `PoliticaDescuentoCredito`. Y entra `ServicioVentaConvenio`, que recibe la politica y usa su resultado.

**Como se relaciona**

`Program` (Composition Root) registra en el conjunto las estrategias disponibles. Al atender una venta con convenio, `ServicioVentaConvenio` selecciona la implementacion de `IPoliticaConvenio` segun el convenio del cliente y la invoca. La venta normal no participa y queda como estaba. No interactua con otro patron adoptado en este punto: es un mecanismo independiente de variacion.

**Impacto**

Clases creadas: `IPoliticaConvenio`, `PoliticaSoloDescuento`, `PoliticaSoloCredito`, `PoliticaDescuentoCredito`, `ServicioVentaConvenio` (y el tipo `SolicitudConvenio`). Clases modificadas: `Program` (registro de estrategias y punto de entrada de la nueva venta). Clases eliminadas: ninguna. Efecto sobre las solicitudes del Anexo B: habilita SC-3 sin tocar la venta normal; SC-1 y SC-2 no se ven afectados.

**Que cuesta**

Se paga mas indireccion: una interfaz, tres estrategias y un servicio nuevo, ademas del registro en `Program`. La venta con convenio agrega una capa que la venta simple no tiene, y depurar una politica requiere seguir el contrato. Es el costo de tener un punto de variacion real, que solo se justifica porque SC-3 lo exige.

**Origen**

Propuesta de la herramienta aceptada tras verificarla contra el codigo. Queda registrada en la bitacora como **B-03** (Strategy para las politicas de convenio) y complementada por **B-04** (no crear una estrategia por entidad, sino por tipo de calculo) y **B-05** (rechazo de la Facade adicional).
