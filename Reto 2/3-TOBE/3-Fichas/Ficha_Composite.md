# Ficha de patrón: Composite

## Patrón y punto de dolor que resuelve

P-03: `ServicioMonitoreoProductos.cs:20-47` reúne stock y vencimiento; `Program.cs:62-87` compone los eventos. Una alerta con evento propio obliga a modificar monitor y `Program` y a crear el evento.

## Alternativas evaluadas

**No hacer nada:** el monitor crece por regla. **Lista simple con coordinador:** viable; también separa reglas. Se elige un contrato común para invocar el grupo actual y sus hojas. **Chain of Responsibility:** podría ejecutar todos los manejadores, pero no se necesita decisión de continuidad ni enlaces entre reglas. **Composite:** adoptado en su forma mínima; el coordinador implementa la misma interfaz, sin clases adicionales frente a esa lista con coordinador.

## Qué sale y qué entra

Sale `ServicioMonitoreoProductos`. Entran `IReglaAlerta` como componente, `ReglaStockMinimo` y `ReglaVencimiento` como hojas, y `MonitorCompuesto` como compuesto. Los tres implementan `Verificar(IEnumerable<Producto>)`. El compuesto contiene una colección ordenada de componentes. Se conservan ambos eventos y Observer.

## Cómo se relaciona

`Program` construye hojas y grupo, conecta los eventos e invoca el grupo mediante `IReglaAlerta`. Stock recorre todos los productos antes de ejecutar vencimiento. Así se conservan orden, textos y colores. El mismo contrato representa una comprobación individual y la comprobación conjunta ya existente. No se añaden grupos anidados hipotéticos ni se afirma que una lista sea incapaz de resolver P-03. Comparte únicamente la composición con Strategy y Template Method.

## Impacto

Entran tres clases y una interfaz; sale un servicio y cambia `Program`. Por alerta con evento propio: antes, dos archivos existentes modificados y uno nuevo; después, un existente (`Program`) y dos nuevos (hoja y evento). El total sigue siendo tres. No cambian el compuesto ni las reglas anteriores. No incorpora funciones de SC-1, SC-2 o SC-3.

## Qué cuesta

Las reglas se distribuyen en hojas; el registro en `Program` sigue siendo necesario. Frente a la lista simple, se añade el compromiso de que el coordinador cumpla la interfaz. La uniformidad es el beneficio específico; el ahorro consiste en modificar menos código existente, no en reducir archivos totales.

## Origen

B-07 registra la elección del equipo. Esta revisión asistida añade la comparación con lista simple y precisa costos y límites; debe reflejarse en la bitácora en preparación.
