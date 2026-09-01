# Reto 2 - Analisis de puntos de dolor

## Alcance y criterio

Este analisis toma como AS-IS la solucion de `Trabajo Farmacia/03-Src`. Solo incluye rigideces relacionadas con creacion, composicion y coordinacion de objetos; no repite defectos funcionales del Reto 1. El costo se mide contando los archivos y clases existentes que hoy deben abrirse o modificarse para atender el escenario descrito.

| ID | Punto de dolor | Costo actual | Prioridad | Decision |
|---|---|---:|---|---|
| P-01 | Ensamblaje manual concentrado en `Program` | 1 archivo / 1 clase implicita, 13 construcciones y 4 suscripciones | Alta | Intervenir |
| P-02 | La venta no admite politicas comerciales variables | 4 archivos / 4 clases | Alta | Intervenir |
| P-03 | Cada nueva alerta amplia deteccion, evento y composicion | 3 archivos / 3 clases por alerta | Alta | Intervenir |
| P-04 | El algoritmo de carga TXT esta triplicado | 3 archivos / 3 clases | Media | Intervenir |
| P-05 | El menu esta centralizado en un `switch` | 1 archivo / 1 clase implicita | Baja | **No intervenir** |

## P-01 - Ensamblaje manual concentrado en `Program`

**Donde:** `AppFarmaciaConsola/Program.cs:8-63` crea tres creadores, un diccionario, un selector, tres cargadores y seis servicios. Las lineas `67-110` conectan manualmente cuatro eventos con la consola.

**Detalle del codigo:** las lineas `17-23` registran cada tipo de producto por una clave textual; `25-31` construyen selector y cargador; `39-63` construyen el grafo de servicios. Las lineas `67`, `78`, `89` y `100` conocen los canales concretos y sus eventos.

**Sintoma:** para comprender como colaboran las interfaces es obligatorio recorrer el inicio completo de `Program`. La construccion y la presentacion comparten el mismo archivo, por lo que agregar una colaboracion aumenta un punto de entrada que ya contiene 13 expresiones `new` y 4 suscripciones.

**Escenario y costo:** incorporar un nuevo servicio transversal obliga a abrir y modificar `Program.cs`: **1 archivo / 1 clase implicita**. Aunque el conteo es bajo, el riesgo es alto porque todas las dependencias convergen en sus primeras 110 lineas.

**Por que importa:** es el punto de ensamblaje mencionado expresamente por el Reto 2. Debe hacerse legible sin introducir un contenedor automatico ni cambiar el estilo arquitectonico.

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

**Por que importa:** el costo se repite por cada regla y el monitor crece horizontalmente. Permite evaluar una coordinacion componible, por ejemplo `Chain of Responsibility`, sin alterar las alertas actuales.

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

Se priorizan P-01 a P-04 porque su costo se repite o afecta el flujo central y la solicitud SC-3. P-05 queda documentado como deuda aceptada: demuestra que el equipo no adopta un patron solo por ser una buena practica, sino cuando el beneficio medido supera su costo.
