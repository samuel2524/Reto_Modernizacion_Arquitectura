# Ficha de patron - Template Method

**Patron y punto de dolor que resuelve**

`Template Method` responde a **P-04** (el algoritmo de carga TXT esta triplicado). La evidencia esta en la misma estructura repetida de `CargadorProductosTxt.cs:22-63`, `CargadorClientesTxt.cs:13-43` y `CargadorUsuariosTxt.cs:13-45`: los tres verifican `File.Exists`, leen las lineas, recorren, aplican `Split(';')`, construyen una entidad, la agregan al destino y capturan la excepcion. Solo cambia la conversion de una fila a su entidad.

**Alternativas que evaluamos**

1. **No hacer nada** (descartada): una regla comun de formato (aceptar encabezados, ignorar comentarios) seguiria teniendo que implementarse y mantenerse sincronizada en tres archivos separados, con riesgo de divergir.
2. **Una utilidad estatica compartida** (descartada): concentraria la lectura en un helper, pero obligaria a cada cargador a recordar llamar a cada paso; un consumidor que salte un paso escala mal y reparte el orden logico del algoritmo fuera del flujo.
3. **Template Method** (adoptada): una clase base `CargadorTxt<T>` fija el flujo completo en un solo lugar y deja el parseo de cada linea como paso especializado (`ParsearLinea`).

**Que sale y que entra**

*Sale:* la duplicacion del flujo de lectura y manejo de errores en los tres cargadores.

*Entra:* `CargadorTxt<T>` (clase base que contiene el algoritmo comun: validar, leer, recorrer, parsear, agregar, manejar error) y el paso abstracto `ParsearLinea(string linea)` que cada cargador concreto implementa. `CargadorProductosTxt`, `CargadorClientesTxt` y `CargadorUsuariosTxt` pasan a heredar de la base y solo conservan su conversion de fila.

**Como se relaciona**

`Program` sigue construyendo los cargadores concretos tal como hoy, porque cada uno conserva su interfaz (ICargadorProductos, ICargadorClientes, ICargadorUsuarios). Al llamar `Cargar`, la clase base ejecuta el flujo comun y delega en `ParsearLinea`. No interactua con otro patron adoptado; es una reorganizacion interna de la carga. Se cuida que el paso de la plantilla sea comun para todos y no se agreguen pasos vacios que una subclase no pueda cumplir.

**Impacto**

Clases creadas: `CargadorTxt<T>`. Clases modificadas: `CargadorProductosTxt`, `CargadorClientesTxt`, `CargadorUsuariosTxt` (pasan a heredar y recortan su metodo al parseo). Clases eliminadas: ninguna interfaz: las tres interfaces ICargador* se conservan para no alterar a sus consumidores. Efecto sobre las solicitudes del Anexo B: un cambio transversal de formato se hace una vez en la base; SC-1 (nuevos tipos de producto) se simplifica porque el parseo de fila queda aislado en cada creador o en el cargador concreto.

**Que cuesta**

Se paga una clase base nueva y la transformacion de los tres cargadores. El flujo ahora vive en la herencia, lo que exige disciplina: si una subclase necesita un paso distinto, modificar la plantilla afecta a los tres, por lo que los pasos deben seguir siendo comunes. Es el costo de eliminar la triple copia.

**Origen**

Propuesta de la herramienta aceptada tras verificarla contra el codigo. Queda registrada en la bitacora como **B-08** (Template Method para la repeticion de los tres cargadores TXT).
