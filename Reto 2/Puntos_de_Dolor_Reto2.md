# Reto 2 - Analisis de puntos de dolor

## Alcance y criterio

Este analisis toma como AS-IS la solucion de `Trabajo Farmacia/03-Src`. Solo incluye rigideces relacionadas con creacion, composicion y coordinacion de objetos; no repite defectos funcionales del Reto 1. El costo se mide contando los archivos y clases existentes que hoy deben abrirse o modificarse para atender el escenario descrito.

| ID | Punto de dolor | Costo actual | Prioridad | Decision |
|---|---|---:|---|---|
| P-01 | La creacion de medicamentos usa dos mecanismos divergentes | 2 archivos / 2 clases | Media | Consolidar Factory Method existente |
| P-02 | La venta no admite politicas comerciales variables | 4 archivos / 4 clases | Alta | Intervenir |
| P-03 | Cada nueva alerta amplia deteccion, evento y composicion | 3 archivos / 3 clases por alerta | Alta | Intervenir |
| P-04 | El algoritmo de carga TXT esta triplicado | 3 archivos / 3 clases | Media | Intervenir |
| P-05 | El menu esta centralizado en un `switch` | 1 archivo / 1 clase implicita | Baja | **No intervenir** |

## P-01 - Creacion de medicamentos con mecanismos divergentes

**Donde:** `Factories/CreadorMedicamentoCapsula.cs:10-26` crea capsulas desde `DatosProducto`; `Factories/ProductoFactory.cs:13-42` ofrece otra creacion estatica para capsulas y liquidos. La carga real usa `CargadorProductosTxt.cs:47-53`, `ISelectorCreadorProducto` e `ICreadorProducto`; `ProductoFactory` no tiene consumidores.

**Detalle del codigo:** `CreadorMedicamentoCapsula.cs:22-24` usa stock minimo y vencimiento del archivo. En cambio, `ProductoFactory.cs:23-26` fija stock minimo en 5, vencimiento a seis meses y relleno gel. Ambos caminos pueden construir el mismo tipo con reglas diferentes.

**Sintoma:** una modificacion en la politica de creacion de capsulas obliga a revisar dos mecanismos para evitar resultados inconsistentes. La presencia de una fabrica estatica desconectada tambien dificulta saber cual es la ruta oficial de creacion.

**Escenario y costo:** cambiar vencimiento, stock minimo o datos del laboratorio exige revisar `CreadorMedicamentoCapsula` y `ProductoFactory`: **2 archivos / 2 clases**.

**Por que importa:** el TO-BE debe consolidar `ICreadorProducto` como unico mecanismo y eliminar `ProductoFactory`. Factory Method ya existe en el AS-IS; se mantiene y consolida, pero no se presenta como patron nuevo del Reto 2.

## P-02 - La venta no admite politicas comerciales variables

**Donde:** `BibFarmacia/Servicios/ServicioVenta.cs:13-22` fija sus dos colaboradores y `35-52` fija toda la secuencia de venta. `Interfaces/IDescuento.cs:9-12` y `Servicios/ServicioDescuento.cs:11-16` definen un descuento aislado que no participa en esa secuencia. `Program.cs:51-60` tampoco lo conecta.

**Detalle del codigo:** `ServicioVenta.cs:39` descuenta inventario, `41-46` crea el movimiento y `48-50` lo registra. No existe un punto donde elegir una regla comercial antes de completar la operacion. `ServicioDescuento.cs:15` deja fijo un unico 10 %, sin contexto de convenio.

**Sintoma:** SC-3 exige seleccionar descuentos y credito segun empresa, banco, cooperativa o institucion. Hoy esa variacion tendria que incorporarse dentro de `ServicioVenta` o mediante condicionales externos.

**Escenario y costo:** para conectar siquiera el descuento existente hay que estudiar o modificar `ServicioVenta`, `IDescuento`, `ServicioDescuento` y `Program`: **4 archivos / 4 clases**. Cada convenio agregado aumentaria las ramas del flujo central si no se crea un punto de variacion.

**Por que importa:** afecta el caso de uso central y es el punto mas directamente relacionado con SC-3. Es candidato natural para evaluar `Strategy`, pero la decision se documentara comparandola con alternativas.

## P-03 - Cada nueva alerta amplia deteccion, evento y composicion

**Donde:** `ServicioMonitoreoProductos.cs:8-18` crea dos eventos concretos; `20-31` implementa stock minimo y `33-47` vencimiento. `Program.cs:62-87` construye el monitor y suscribe ambos canales.

**Detalle del codigo:** cada regla tiene su propio recorrido de productos y dispara un tipo concreto en `ServicioMonitoreoProductos.cs:28` o `44-45`. La salida debe conectarse otra vez en `Program.cs:67-87`.

**Sintoma:** agregar una alerta de sobrestock obliga a crear otro evento, ampliar el monitor y ampliar el ensamblaje. Deteccion, publicacion y presentacion cambian juntas.

**Escenario y costo:** una alerta nueva requiere un archivo de evento, `ServicioMonitoreoProductos.cs` y `Program.cs`: **3 archivos / 3 clases**.

**Por que importa:** el costo se repite por cada regla y el monitor crece horizontalmente. `Composite` permite agrupar reglas independientes y ejecutarlas todas sin alterar las alertas actuales.

## P-04 - El algoritmo de carga TXT esta triplicado

**Donde:** `CargadorProductosTxt.cs:22-63`, `CargadorClientesTxt.cs:13-43` y `CargadorUsuariosTxt.cs:13-45` repiten la misma estructura de control.

**Detalle del codigo:** los tres verifican `File.Exists`, ejecutan `File.ReadAllLines`, recorren lineas, aplican `Split(';')`, crean una entidad, la agregan al destino y capturan `Exception`. Solo varia la conversion de una fila.

**Sintoma:** una regla comun de formato debe implementarse tres veces. Por ejemplo, aceptar encabezados o ignorar comentarios exige mantener sincronizados tres algoritmos casi iguales.

**Escenario y costo:** ese cambio obliga a modificar los tres cargadores: **3 archivos / 3 clases**.

**Por que importa:** la repeticion encarece cambios transversales y puede producir comportamientos distintos entre archivos. Es un candidato para evaluar `Template Method`, conservando especializado solamente el parseo.

## P-05 - Menu centralizado en un `switch` - NO SE INTERVIENE

**Donde:** `AppFarmaciaConsola/Program.cs:203-216` declara y selecciona siete opciones; `218-413` contiene sus flujos completos.

**Detalle del codigo:** cada opcion es una rama del mismo `switch`. Agregar "Ver movimientos" requiere una etiqueta y un `case`, pero no obliga a cambiar servicios existentes si la consulta ya esta disponible.

**Escenario y costo actual:** agregar esa opcion cuesta **1 archivo / 1 clase implicita**.

**Razon para descartarlo:** aplicar `Command` literalmente exigiria como minimo `IComandoMenu`, siete comandos para las opciones actuales y modificar `Program`: **9 archivos / 9 clases**. El sistema tiene un solo menu y no necesita deshacer, historial, atajos ni varias interfaces. El remedio multiplica por nueve el costo estructural para resolver un cambio que hoy es local; por eso se acepta conscientemente esta rigidez.

## Conclusion

P-01 consolida un Factory Method ya existente; P-02, P-03 y P-04 justifican respectivamente Strategy, Composite y Template Method como patrones nuevos. P-05 queda como deuda aceptada: demuestra que el equipo no adopta un patron cuando su costo supera el beneficio.
