# ADR-006 — Seleccion del creador por tipo de producto e implementacion de SC-1

- **Estado:** Aceptado
- **Fecha:** 2026-08-08 · **Revisado:** 2026-08-08 (contrato de datos)
- **Relacionado:** ADR-002, ADR-003, PD-03, H-02, H-06 y SC-1
- **Sustituye el alcance diferido de:** ADR-003, seccion "Alcance de OCP"

## Contexto

ADR-003 introdujo `ICreadorProducto`, `DatosProducto` y
`CreadorMedicamentoCapsula`, y dejo explicitamente fuera de alcance el
discriminador de tipo:

> "todavia no podemos cargar varios tipos diferentes desde el mismo TXT porque
> el archivo actual no indica que tipo corresponde a cada fila. Esa seleccion se
> resolvera cuando SC-1 permita modificar el formato de entrada"

Ese momento llego. La Fase 4 exige implementar una de las tres solicitudes de
cambio, y el equipo eligio **SC-1: vender cosmeticos y productos comestibles**.
SC-1 autoriza modificar el formato de `productos.txt`, que era la condicion que
ADR-003 puso para resolver la seleccion.

Con un unico `ICreadorProducto` inyectado, `CargadorProductosTxt` no puede
decidir por fila que construir. Implementar SC-1 sin esta decision obligaria a
modificar el cargador, y la metrica del criterio 5 no llegaria a cero.

## Objetivo

Que agregar un tipo de producto no modifique ninguna clase existente.

## Alternativas evaluadas

### 1. Un `if` o `switch` por tipo dentro de `CargadorProductosTxt`

Es lo mas directo y no agrega tipos nuevos. Se descarta porque cada producto
futuro volveria a modificar el cargador: es exactamente el defecto H-06 que el
rediseño busca eliminar, movido de sitio. La metrica de SC-1 quedaria en una
clase modificada en vez de cero.

### 2. Un cargador TXT por tipo de producto

`CargadorCosmeticosTxt`, `CargadorComestiblesTxt`, cada uno con su archivo. Evita
el discriminador, pero duplica en cada cargador la lectura, el delimitador y las
conversiones, que es la responsabilidad que ADR-002 acababa de concentrar en un
solo sitio. Ademas multiplica los archivos de datos y el cableado del arranque.

### 3. Selector de creadores por clave, resuelto en el composition root

Una abstraccion `ISelectorCreadorProducto` que, dado el tipo leido de la fila,
devuelve el `ICreadorProducto` correspondiente. La implementacion mantiene un
diccionario que se llena en `Program`. El cargador pasa a depender del selector
en lugar de un creador concreto.

Agregar un tipo es: una clase de dominio, un creador y una linea de registro.
Ninguna clase existente cambia.

### 4. Ampliar `DatosProducto` con los campos propios de cada tipo

Propuesta durante la implementacion: agregar `Marca`, `Presentacion` y
`Categoria` al DTO, junto a los campos comunes.

**Se descarta.** `DatosProducto` se convertiria en la union de los campos de
todos los tipos existentes y futuros, de modo que cada tipo nuevo obligaria a
modificarlo:

| Al agregar | Habria que modificar |
| --- | --- |
| `Cosmetico` | `DatosProducto` (+`Marca`, +`Presentacion`) |
| `Comestible` | `DatosProducto` (+`Categoria`) |
| el siguiente tipo | `DatosProducto` otra vez |

Es el mismo defecto de la alternativa 1 desplazado del cargador al DTO: el
sistema seguiria cerrado a la extension y la metrica del criterio 5 quedaria en
**una clase modificada en vez de cero**, incumpliendo los criterios de
aceptacion 1 y 5 de este mismo ADR. Se resuelve con el contrato de datos que se
define abajo.

## Decision

Adoptar la alternativa 3 e introducir:

- `ISelectorCreadorProducto`, con `ICreadorProducto Seleccionar(string tipo)`.
- `SelectorCreadorProducto`, con un `IDictionary<string, ICreadorProducto>`
  poblado en el composition root.
- `Cosmetico` y `Comestible`, subtipos **directos** de `Producto` (no de
  `Medicamento`: no tienen laboratorio).
- `CategoriaComestible` como enumeracion: `Gaseosa`, `Agua`, `Helado`, `Snack`.
- `CreadorCosmetico` y `CreadorComestible`, implementaciones de `ICreadorProducto`.

`CargadorProductosTxt` deja de recibir un `ICreadorProducto` y recibe un
`ISelectorCreadorProducto`. Es el unico cambio sobre lo aprobado en ADR-003, y
se hace **antes** de que exista codigo, no sobre codigo ya escrito.

## Contrato de datos

Esta seccion resuelve el vacio que bloqueo la implementacion: ADR-003 y la
version inicial de este ADR definian el discriminador de tipo pero no de donde
salen los campos propios de cada producto.

### Estructura de `DatosProducto`

El DTO transporta **los campos comunes ya interpretados** mas **las columnas
propias del tipo sin interpretar**:

```text
DatosProducto
  + Tipo: string
  + Nombre: string
  + Precio: decimal
  + Stock: int
  + StockMinimo: int
  + FechaVencimiento: DateTime
  + Extra: IReadOnlyList<string>
```

`Extra` contiene las columnas restantes de la fila, en orden y sin convertir.
Cada creador interpreta unicamente las suyas:

| Creador | Lee de `Extra` |
| --- | --- |
| `CreadorMedicamentoCapsula` | `[0]` -> nombre del laboratorio |
| `CreadorCosmetico` | `[0]` -> `Marca` · `[1]` -> `Presentacion` |
| `CreadorComestible` | `[0]` -> `Categoria`, convertida a `CategoriaComestible` |

Agregar un tipo es una clase de dominio, un creador y una linea de registro.
`DatosProducto`, `CargadorProductosTxt` y `SelectorCreadorProducto` no cambian.

### Formato de `productos.txt`

Se agrega una **primera columna con el tipo**. Las columnas comunes conservan su
orden y su significado, desplazadas una posicion; las columnas propias del tipo
van al final:

```text
medicamento;Nombre;Precio;Stock;StockMinimo;Fecha;Laboratorio
cosmetico;Nombre;Precio;Stock;StockMinimo;Fecha;Marca;Presentacion
comestible;Nombre;Precio;Stock;StockMinimo;Fecha;Categoria
```

### Registros de SC-1

Las diez filas actuales solo reciben el prefijo `medicamento;`; no cambia ningun
otro valor. Se agregan siete registros que cubren las cuatro categorias que el
enunciado nombra para SC-1:

```text
cosmetico;ShampooAnticaspa;18000;12;5;2027-03-15;HeadShoulders;Frasco400ml
cosmetico;CremaHidratante;25000;8;5;2027-06-30;Nivea;Pote200ml
cosmetico;LabialMate;15000;20;5;2028-01-20;Vogue;Barra
comestible;CocaCola350;3500;40;10;2026-05-10;Gaseosa
comestible;AguaCristal600;2500;60;15;2027-01-01;Agua
comestible;HeladoVainilla;6000;3;5;2026-03-20;Helado
comestible;PapasFritas;4000;30;10;2026-09-12;Snack
```

Son **datos de demostracion**, no requisitos de negocio: el enunciado nombra las
categorias (gaseosas, agua, helados, snacks) pero no productos concretos. Se
declaran aqui para que no aparezcan sin explicacion en la evidencia.

`HeladoVainilla` se define deliberadamente con stock 3 y minimo 5, por debajo del
umbral. Al cargarlo, `ServicioMonitoreoProductos` dispara la alerta de stock
sobre un `Comestible` sin downcast y sin una linea de codigo nueva: es la
verificacion en ejecucion de la afirmacion LSP del TO-BE.

## Consecuencias

### Lo que se gana

Agregar un tipo de producto no modifica ninguna clase. La metrica de SC-1 pasa
de 1-2 clases modificadas a **0 modificadas y 7 creadas**.

### El costo que se acepta

1. **Una indireccion mas.** Leer una fila ahora pasa por cargador, selector y
   creador. Se acepta porque cada salto tiene un motivo de cambio distinto y se
   puede probar por separado.
2. **`Extra` viaja sin interpretar.** Rompe parcialmente la promesa de ADR-003 de
   que el DTO "transporta unicamente valores ya interpretados". Es inevitable:
   solo el creador sabe que la septima columna de un comestible es un
   `CategoriaComestible`. La alternativa seria un DTO por tipo, y entonces
   `ICreadorProducto.Crear` no podria tener una firma unica. Se acepta el costo
   menor: los campos comunes siguen interpretados y solo los propios del tipo
   viajan como texto.
3. **El diccionario del selector es configuracion en codigo.** Un tipo mal
   escrito en el archivo falla en tiempo de ejecucion, no de compilacion. Se
   acepta: el sistema ya falla en ejecucion ante una fila mal formada, y el
   comportamiento observable ante error no cambia.
4. **Migracion del archivo de datos.** El `productos.txt` actual necesita la
   columna nueva. Es la unica modificacion externa al codigo, y SC-1 la
   autoriza expresamente.
5. **Se rompe el paralelismo con los otros dos cargadores.** `CargadorClientesTxt`
   y `CargadorUsuariosTxt` siguen sin selector, porque no tienen variantes. Se
   acepta antes que introducir una simetria que nadie necesita.

## Principios aplicados

- **OCP:** el cargador, el selector y el DTO quedan cerrados a modificacion y el
  sistema abierto a nuevos tipos por extension. Es la evidencia empirica que pide
  el criterio 5.
- **DIP:** `CargadorProductosTxt`, de alto nivel respecto a la construccion,
  depende de `ISelectorCreadorProducto`; las implementaciones concretas se
  resuelven en `Program`.
- **SRP:** interpretar el archivo, elegir el creador y construir el producto son
  tres motivos de cambio distintos en tres componentes distintos.
- **LSP:** `Cosmetico` y `Comestible` se observan a traves de `Producto` sin
  downcast. Cumplen el mismo contrato verificado en la matriz LSP del TO-BE:
  `Stock` y `StockMinimo` conservan su significado, `FechaVencimiento` se compara
  igual y ningun subtipo redefine la transicion de `Stock`.

## Comportamiento preservado

Las diez filas actuales de `productos.txt` llevaran `medicamento` como tipo y
seguiran construyendose con `CreadorMedicamentoCapsula`: mismo laboratorio
`"Medellin"` y `"4444444"`, mismo `TipoRelleno.Gel`, mismos stock minimo y fecha
tomados del archivo. Mensajes, orden, duplicados y fallas parciales no cambian.

**Los casos de caracterizacion necesitan las dos versiones del archivo.** El
sistema original lee el formato sin la columna de tipo; el rediseñado, el formato
con ella. Hay que conservar el `productos.txt` original para ejecutar el sistema
AS-IS y comparar que las diez filas produzcan salida identica. Si se le entrega
el archivo nuevo al sistema original, `datos[5]` deja de ser el laboratorio y
todas las columnas se corren una posicion: la comparacion seria invalida.

Las filas de cosmeticos y comestibles no participan en la comparacion, porque no
existen en el sistema original. Se prueban aparte, como evidencia de SC-1.

## Criterios de aceptacion

1. Agregar un tipo de producto no modifica ninguna clase existente.
2. `CargadorProductosTxt` no conoce ningun constructor concreto de producto.
3. El selector no lee archivos ni convierte textos.
4. Las diez filas actuales siguen siendo `MedicamentoCapsula` con los mismos valores.
5. La tabla del criterio 5 muestra 0 clases modificadas frente a 1-2 en el AS-IS.
6. `DatosProducto` no cambia al agregar `Cosmetico`, `Comestible` ni ningun tipo
   posterior.

## Impacto en el UML TO-BE

Se muestran `ISelectorCreadorProducto`, `SelectorCreadorProducto`,
`CreadorCosmetico`, `CreadorComestible`, `Cosmetico`, `Comestible` y
`CategoriaComestible`, y la dependencia de `CargadorProductosTxt` hacia el
selector. Todos en color de OCP, porque ninguno existia y ninguno obliga a
modificar una clase previa.

`DatosProducto` se muestra con `Tipo` y `Extra`, sin campos propios de ningun
tipo concreto.
