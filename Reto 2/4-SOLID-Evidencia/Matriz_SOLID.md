# Matriz de verificacion SOLID - TO-BE Reto 2

Verificacion de que los tres patrones adoptados en el Reto 2 (Strategy, Composite y Template Method) no rompen los principios SOLID ya pagados en el Reto 1. Factory Method sigue siendo parte del AS-IS: no se cuenta como patron incorporado en este reto (ver Analisis_de_Patrones.md, "Decision final") y por eso no tiene fila propia aqui. Cada fila es un patron y cada columna un principio. Valores: **Refuerza**, **Neutro**, **Tensionado pero compensado**, **Roto**. Toda celda distinta de Neutro lleva su linea de evidencia debajo. Ninguna celda queda en Roto.

## Matriz

| Patron adoptado | SRP | OCP | LSP | ISP | DIP |
|---|---|---|---|---|---|
| **Strategy** (`IPoliticaConvenio`) | Refuerza | Refuerza | Neutro | Tensionado pero compensado | Refuerza |
| **Composite** (`IReglaAlerta`) | Refuerza | Refuerza | Tensionado pero compensado | Neutro | Refuerza |
| **Template Method** (`CargadorTxt<T>`) | Neutro | Refuerza | Tensionado pero compensado | Neutro | Neutro |

## Estrategia (Strategy) - P-02 / SC-3

### SRP - Refuerza
Cada estrategia concreta (`PoliticaSoloDescuento`, `PoliticaSoloCredito`, `PoliticaDescuentoCredito`) tiene una sola responsabilidad: calcular su criterio comercial. Antes, esa variacion tenderia a caer dentro de `ServicioVenta.Vender` (`ServicioVenta.cs:35-52`), que ya hace tres cosas. Ahora el calculo queda aislado en su propia clase.

### OCP - Refuerza
Agregar una politica nueva es escribir una clase que implemente `IPoliticaConvenio` y registrarla en `Program`. No se modifica `ServicioVenta`, ni el flujo normal, ni la salida de los 12 casos. Es el punto de variacion que P-02 declara, sin condicionales nuevos.

### LSP - Neutro
Las tres estrategias implementan el mismo contrato y son sustituibles entre si: quien consume `IPoliticaConvenio` no distingue cual se inyecto. No hay herencia con pasos vacios ni contratos que una estrategia no pueda cumplir.

### ISP - Tensionado pero compensado
`Evaluar(SolicitudConvenio)` devuelve varios datos (subtotal, descuento, total, credito, cupo, motivo). El contrato es un poco amplio. **Compensacion**: el equipo lo declara y lo acota en el analisis (Ficha_Strategy): el contrato se limita a la evaluacion comercial y no mezcla responsabilidades de persistencia ni de interfaz. Queda documentado como límite para no terminar con una interfaz demasiado amplia.

### DIP - Refuerza
`ServicioVentaConvenio` depende de la abstraccion `IPoliticaConvenio`, no de una estrategia concreta. `Program` (Composition Root) decide que implementacion inyectar, manteniendo la venta dependiente del contrato tal como se declaro en el analisis SC-3.

## Compuesto (Composite) - P-03

### SRP - Refuerza
`ReglaStockMinimo` y `ReglaVencimiento` encapsulan cada una su propia logica de deteccion, que antes vivia como metodos separados en `ServicioMonitoreoProductos.cs:20-31` y `33-48`. Cada regla tiene ahora una sola responsabilidad y el compuesto solo las coordina.

### OCP - Refuerza
Una alerta nueva (p. ej. sobrestock) es una clase hoja nueva y su registro en `Program`. No se amplia el monitor ni se rehace la composicion, que es justo la rigidez que P-03 declara.

### LSP - Tensionado pero compensado
Todas las reglas deben poder tratarse de forma uniforme por `MonitorCompuesto`: se ejecutan todas y cada una decide si dispara. **Compensacion**: se evita que una hoja introduzca pasos o contratos que otras no puedan cumplir. La verificacion se apoya en el `Observer` ya existente, que sigue publicando igual. Se revisa en la matriz que ninguna regla rompa el contrato comun.

### ISP - Neutro
`IReglaAlerta` expone una sola operacion de evaluacion; las hojas no cargan con metodos que no usan.

### DIP - Refuerza
`MonitorCompuesto` depende del contrato `IReglaAlerta` para componer las reglas. `Program` arma el compuesto igual que hoy arma el monitor (Program.cs:62-87), sin que el consumidor conozca las hojas concretas.

## Plantilla (Template Method) - P-04

### SRP - Neutro
La clase base `CargadorTxt<T>` concentra el algoritmo de carga y cada subclase solo el parseo. No se sobrecarga ninguna clase: se mantiene la separacion que ya habia (un cargador por origen).

### OCP - Refuerza
Un cargador nuevo o una regla general del formato se atiende en un solo lugar. `CargadorProductosTxt`, `CargadorClientesTxt` y `CargadorUsuariosTxt` heredan de la base y solo implementan `ParsearCampos`. El algoritmo comun (validar, leer, recorrer, split, agregar, error) deja de duplicarse en tres archivos (antes en `CargadorProductosTxt.cs:22-63`, `CargadorClientesTxt.cs:13-43`, `CargadorUsuariosTxt.cs:13-45`).

### LSP - Tensionado pero compensado
El riesgo clasico del patron es que una subclase no pueda cumplir un paso de la plantilla (p. ej. un paso vacio) y deje de ser sustituible. **Compensacion**: los pasos de la plantilla deben ser comunes a los tres cargadores; si un dia un cargador necesita un paso distinto, no se agrega un metodo vacio a la base; se redefine en la subclase o se ajusta la conversion `ParsearCampos`. Queda declarado y mitigado en la Ficha_TemplateMethod y en el analisis SC-3.

### ISP - Neutro
Las interfaces de consumo (`ICargadorProductos`, `ICargadorClientes`, `ICargadorUsuarios`) se conservan intactas y siguen siendo la unica exposicion para sus consumidores.

### DIP - Neutro
`Program` y los servicios siguen dependiendo de las interfaces `ICargador*`, no de la clase base. La herencia es interna a cada cargador; la abstraccion que ven los consumidores no cambia.

## Nota sobre Factory Method

Factory Method (`ICreadorProducto`, `SelectorCreadorProducto`) no aparece en la matriz porque no es un patron incorporado en el Reto 2: viene del Reto 1 y se conserva sin cambios como parte del AS-IS (Analisis_de_Patrones.md, "Decision final"). No esta atado a ningun punto de dolor de este reto (P-01 se resuelve sin patron, ver Puntos_de_Dolor.md y Tabla_Cambio_Estructural.md), asi que no hay una verificacion SOLID nueva que hacerle aqui.

## Errores tipicos verificados (seccion 5 del enunciado)

| Situacion que evita el enunciado | Que principio protege | Estado en el TO-BE |
|---|---|---|
| Una fabrica que crece con un condicional por tipo nuevo | OCP | No ocurre: el selector usa un diccionario (`SelectorCreadorProducto.cs`), no condicionales. |
| Una fachada que absorbe logica de negocio | SRP | No se adopta Facade (descartada en la bitacora B-05/B-06). |
| Un punto de acceso global de instancia unica | DIP/testabilidad | No se usa Singleton. |
| Un metodo plantilla con pasos que alguna subclase no cumple | LSP | Declarado y compensado en Template Method (ver celda LSP). |
| Una envoltura que cambia el contrato de lo que envuelve | LSP | Las interfaces ICargador* se conservan sin cambios. |
