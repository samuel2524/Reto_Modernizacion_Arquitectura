# Vista para el equipo de desarrollo

Para: el ingeniero que entra al equipo dentro de seis meses y tiene que
hacer un cambio sin romper nada. No estuvo en ninguna reunión de diseño.

## 1. Qué patrones hay, dónde vive cada uno y cómo se relacionan

**Strategy (SC-3, convenios)** vive en `BibFarmacia/Servicios/` e
`Interfaces/`. `IPoliticaConvenio` es el contrato (`Evaluar(SolicitudConvenio):
ResultadoConvenio`); `PoliticaSoloDescuento`, `PoliticaSoloCredito` y
`PoliticaDescuentoCredito` son las tres implementaciones. `ServicioVentaConvenio`
es el contexto: recibe un diccionario `IDictionary<TipoBeneficioConvenio,
IPoliticaConvenio>`, elige la política según el `TipoBeneficio` del convenio
del cliente y aplica su resultado. Los tipos de datos (`Convenio`,
`TipoBeneficioConvenio`, `SolicitudConvenio`, `ResultadoConvenio`) están en
`BibFarmacia/Clases/` y `Enums/`.

**Composite (alertas de stock y vencimiento)** vive en `BibFarmacia/Servicios/`.
`IReglaAlerta` es el contrato común (`Verificar(IEnumerable<Producto>): void`);
`ReglaStockMinimo` y `ReglaVencimiento` son las hojas; `MonitorCompuesto`
agrupa una lista de `IReglaAlerta` y también implementa `IReglaAlerta`, por
eso `Program` lo consume igual que consumiría una regla suelta sin saber que
por dentro reenvía a varias.

**Template Method (carga de TXT)** vive en `BibFarmacia/Servicios/`.
`CargadorTxt<T>` es la clase base con el algoritmo fijo (validar archivo,
leer líneas, recorrer, separar por `;`, agregar al destino, capturar
error); `CargadorProductosTxt`, `CargadorClientesTxt` y `CargadorUsuariosTxt`
heredan de ella y solo implementan `ParsearCampos(string[]): T` y el hook
`MensajeCargaExitosa`.

**Factory Method (consolidado, ya existía en Reto 1)** vive en
`BibFarmacia/Factories/`. `ICreadorProducto` es el contrato,
`CreadorMedicamentoCapsula` / `CreadorCosmetico` / `CreadorComestible` las
implementaciones, `SelectorCreadorProducto` elige cuál usar según el tipo
leído de `productos.txt`. No cambió en este reto.

## 2. Dónde se ensambla el sistema

`AppFarmaciaConsola/Program.cs` sigue siendo el único Composition Root: es
el único archivo que hace `new` de cada servicio concreto, arma el
diccionario de políticas de convenio, arma el `MonitorCompuesto` con sus dos
reglas y conecta todos los eventos. Ningún servicio de `BibFarmacia`
construye a otro directamente. Si algo no arranca, el primer lugar donde
mirar es `Program.cs`.

`Program` depende de `MonitorCompuesto` solo a través de `IReglaAlerta`
(`IReglaAlerta monitorAlertas = monitorCompuesto;`); no conoce las reglas
concretas ni cómo están compuestas.

## 3. Reglas que no se deben romper y por qué

- **La venta normal (`ServicioVenta.Vender`) no se toca.** Es el flujo que
  validan los 12 casos de caracterización (ver `Evidencia_4_2.md`). La venta
  con convenio es una ruta aparte (`ServicioVentaConvenio`) que no pasa por
  `ServicioVenta`.
- **`SolicitudConvenio` y `ResultadoConvenio` solo llevan números y
  banderas, nunca `Cliente`, `Producto` ni `Convenio`.** Si una política
  recibiera esas entidades podría mutarlas y dejaría de ser una estrategia
  de cálculo puro. Quien descuenta stock, registra el movimiento y
  actualiza el cupo es siempre `ServicioVentaConvenio`, nunca la política.
- **El separador `;` y el flujo de lectura viven solo en `CargadorTxt<T>`.**
  Si una subclase vuelve a hacer `Split(';')` por su cuenta, se reintroduce
  la triplicación que Template Method vino a resolver.
- **`ProductoFactory`, `ServicioNotificacion` + `IServicioNotificacion` y
  `AspectoValidacion` no se conectan ni se borran.** Es deuda declarada del
  Reto 1 (sin consumidor, ver H-08); no forman parte del diseño vigente
  pero se conservan para la comparación AS-IS/TO-BE.

## 4. Deuda pendiente

- **`IDescuento` y `ServicioDescuento` se eliminaron** en este reto: cubrían
  el mismo punto de variación comercial que ahora resuelve
  `IPoliticaConvenio`, así que dejaron de tener sentido como deuda
  separada.
- **El formato del archivo de productos sigue acoplado por posición**
  (`datos[0]` es el tipo, `datos[1]` el nombre, etc.). Ningún patrón de
  este reto atiende esto: Factory Method sigue leyendo `Extra[i]` por
  índice en cada creador, igual que en el Reto 1. Agregar una columna
  nueva al inventario sigue costando tocar `DatosProducto`,
  `CargadorProductosTxt` y cada creador: 3 archivos / 3 clases. Es deuda
  declarada, no un punto de dolor con ID propio en esta entrega (no
  confundir con P-01, que ahora es la búsqueda de productos duplicada y ya
  quedó resuelta).
- **El menú de `Program` sigue siendo un único `switch`** (P-05): se evaluó
  y se descartó a propósito resolverlo con un patrón, porque el costo de la
  alternativa (nueve clases) superaba el problema (un archivo).

## 5. Guía de dónde tocar

| Si el cambio es… | Qué crear | Qué modificar | Qué NO tocar |
|---|---|---|---|
| Un tipo de producto nuevo (SC-1: cosméticos, comestibles) | Una clase de dominio que herede de `Producto` + una implementación de `ICreadorProducto` | Un registro nuevo en el diccionario de `SelectorCreadorProducto`, en `Program` | `CargadorProductosTxt`, `DatosProducto`, los demás creadores |
| Un servicio vendible nuevo (SC-2: inyectología, curaciones) | Depende de si se modela como `Producto` (reutiliza la venta) o como un flujo aparte, como se hizo con convenios | El punto de ensamblaje en `Program` para exponer la opción de menú | `ServicioVenta.Vender`, la venta normal existente |
| Un convenio con una entidad nueva (SC-3: otro banco, otra cooperativa) | Nada de código: es un dato, una instancia de `Convenio` con su `TipoBeneficio` y `Porcentaje` | La fuente de datos de clientes (`clientes.txt`), si el convenio se carga desde ahí | `IPoliticaConvenio`, sus tres implementaciones, `ServicioVentaConvenio` |
| Una regla de cálculo comercial nueva (ej. combinar tres beneficios) | Una clase que implemente `IPoliticaConvenio` | Su registro en el diccionario de políticas, en `Program` | `ServicioVentaConvenio`, las demás políticas |
| Una alerta de inventario nueva (ej. sobrestock) | Una clase que implemente `IReglaAlerta` | Su alta (`Agregar(...)`) en `MonitorCompuesto`, en `Program` | `IReglaAlerta`, `ReglaStockMinimo`, `ReglaVencimiento`, el algoritmo de recorrido |
| Una regla de formato nueva para los TXT (ej. ignorar líneas que empiecen con `#`) | Nada nuevo | El algoritmo común en `CargadorTxt<T>.Cargar` | Los tres `ParsearCampos` de las subclases: no deberían saber de esto |

Seis filas, cubriendo las tres solicitudes del Anexo B (SC-1, SC-2, SC-3) y
dos tipos de cambio adicionales típicos sobre los patrones nuevos.
