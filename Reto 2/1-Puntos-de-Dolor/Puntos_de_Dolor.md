# Reto 2 - Analisis de puntos de dolor

## Alcance y criterio

Este analisis toma como AS-IS la solucion de `Trabajo Farmacia/03-Src`. Solo incluye rigideces relacionadas con creacion, composicion y coordinacion de objetos; no repite defectos funcionales del Reto 1. El costo se mide contando los archivos y clases existentes que hoy deben abrirse o modificarse para atender el escenario descrito.

| ID | Punto de dolor | Costo actual | Prioridad | Decision |
|---|---|---:|---|---|
| P-01 | El formato de productos esta acoplado al orden posicional de sus columnas | 3 archivos / 3 clases | Alta | Intervenir |
| P-02 | La venta no admite politicas comerciales variables | 4 archivos / 4 clases | Alta | Intervenir |
| P-03 | Cada nueva alerta amplia deteccion, evento y composicion | 3 archivos / 3 clases por alerta | Alta | Intervenir |
| P-04 | El algoritmo de carga TXT esta triplicado | 3 archivos / 3 clases | Media | Intervenir |
| P-05 | El menu esta centralizado en un `switch` | 1 archivo / 1 clase implicita | Baja | **No intervenir** |

## P-01 - El formato de productos esta acoplado al orden posicional de sus columnas

**Donde:** `CargadorProductosTxt.cs:32-45` lee cada linea con `linea.Split(';')` y construye `DatosProducto` usando posiciones fijas (`datos[0]` a `datos[5]`). Los campos variables de cada tipo de producto se pasan como `datos[6..]` y cada creador los interpreta por indice: `CreadorMedicamentoCapsula.cs:15` usa `datos.Extra[0]`, `CreadorCosmetico.cs:18-19` usa `datos.Extra[0]` y `datos.Extra[1]`, `CreadorComestible.cs:16` usa `datos.Extra[0]`.

**Detalle del codigo:** `DatosProducto.cs:5-11` expone cada campo como propiedad fija y guarda el resto sin nombrar en `Extra: IReadOnlyList<string>`. El cargador conoce el orden exacto de las columnas (`datos[0]` es el tipo, `datos[1]` el nombre, ... `datos[5]` el vencimiento) y delega la interpretacion de los campos restantes por indice en cada creador.

**Sintoma:** agregar un campo nuevo al inventario de productos, por ejemplo la marca del laboratorio o un regimen de precio, obliga a abrir tres lugares a la vez: el DTO para declarar la propiedad, el cargador para leer la posicion de la columna y cada creador que depende de `Extra[i]` por su indice. El formato del archivo queda acoplado a tres clases separadas y una reordenacion de columnas rompe silenciosamente la carga.

**Escenario y costo:** si el inventario agrega un campo (p. ej. marca) se debe modificar `DatosProducto` (declarar la propiedad), `CargadorProductosTxt` (leer la nueva posicion) y ajustar cada creador al desplazamiento de la columna: **3 archivos / 3 clases**. Con los tres tipos actuales (medicamento, cosmético, comestible), el costo crece con cada tipo nuevo que lea columnas por indice.

**Por que importa:** el TO-BE debe encapsular el parseo de cada tipo de producto en su propio creador, de modo que una fila se convierta en `DatosProducto` sin que el cargador tenga que conocer la posicion de cada columna. Ese es el punto donde evaluar Builder o el refuerzo de Factory Method como mecanismo unico de lectura y construccion.

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

P-01 encapsula el parseo del inventario en el creador de cada tipo; P-02, P-03 y P-04 justifican respectivamente Strategy, Composite y Template Method como patrones nuevos. P-05 queda como deuda aceptada: demuestra que el equipo no adopta un patron cuando su costo supera el beneficio.
