# Ficha de patron - Composite

**Patron y punto de dolor que resuelve**

`Composite` responde a **P-03** (cada nueva alerta amplia deteccion, evento y composicion). La evidencia esta en `ServicioMonitoreoProductos.cs:20-31` (`VerificarStock`) y `33-48` (`VerificarVencimiento`): el monitor recorre los productos por separado para cada regla y dispara un evento concreto por regla. Agregar una alerta de sobrestock obliga a crear otro evento, ampliar el monitor y rehacer la composicion en `Program`.

**Alternativas que evaluamos**

1. **No hacer nada** (descartada): cada regla de alerta nueva seguiria tocando deteccion, evento y ensamblaje a la vez, que es el costo repetido que P-03 declara.
2. **Chain of Responsibility** (descartada): una cadena detiene la revisión en el primer manejador que la atiende, pero las alertas de stock y vencimiento deben ejecutarse siempre. No representa bien un grupo de reglas que se recorren completas.
3. **Composite** (adoptada): agrupa las reglas bajo un contrato comun y las ejecuta todas, sin que el monitor crezca con cada regla nueva.

**Que sale y que entra**

*Sale:* el acople de `ServicioMonitoreoProductos` que obliga a meter cada regla como metodo propio dentro del mismo monitor contemplando su evento concreto.

*Entra:* `IReglaAlerta` (contrato comun con su evaluacion), las hojas `ReglaStockMinimo` y `ReglaVencimiento` (cada una encapsula su propia regla y publica su mensaje), y `MonitorCompuesto` (compone las reglas y las ejecuta a todas). El mecanismo `Observer` existente que publica los mensajes se conserva igual, tal como ya esta en el AS-IS.

**Como se relaciona**

`Program` arma el `MonitorCompuesto` y le agrega las reglas, y sigue suscribiendose a la notificacion de cada alerta. Al verificar, el monitor recorre su lista de reglas y cada hoja decide si la dispara. Una alerta nueva es una clase hoja y un registro; no se toca el algoritmo de recorrido ni la composicion de `Program` mas alla del alta. Se apoya en el `Observer` ya existente para publicar, sin introducir un canal nuevo.

**Impacto**

Clases creadas: `IReglaAlerta`, `ReglaStockMinimo`, `ReglaVencimiento`, `MonitorCompuesto`. Clases modificadas: `ServicioMonitoreoProductos` (se transforma: su logica pasa a las reglas) y `Program` (composicion del monitor con reglas). Clases eliminadas: ninguna en el flujo de salida; los eventos y el Observer se conservan. Efecto sobre las solicitudes del Anexo B: no altera la conducta de las alertas existentes; una alerta nueva (p. ej. de sobrestock) se agrega como hoja.

**Que cuesta**

Se paga una clase base (`IReglaAlerta`), dos reglas, el monitor compuesto y la transformacion de `ServicioMonitoreoProductos` y `Program`. La composicion por reglas agrega indireccion: leer donde se decide el umbral ya no es un metodo del monitor sino la regla. Es el costo de dejar de ampliar estructuras cada vez que aparece una regla de alerta.

**Origen**

Propuesta de la herramienta corregida y adoptada tras verificarla. Queda registrada en la bitacora como **B-07** (la herramienta sugirio Chain of Responsibility; el equipo la reemplazo por Composite porque las alertas deben ejecutarse todas).
