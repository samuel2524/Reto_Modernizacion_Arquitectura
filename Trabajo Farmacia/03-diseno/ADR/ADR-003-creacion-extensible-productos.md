# ADR-003 — Hacer extensible la creación de tipos de producto

- **Estado:** Aceptado | 
- **Relacionado:** PD-03, ADR-002 y SC-1

## Contexto
ADR-002 traslada lectura, columnas y conversiones del TXT a
`CargadorProductosTxt`. Sin otra separación, ese adaptador continuaría
conociendo el constructor concreto de `MedicamentoCapsula`.

La carga actual siempre crea cápsulas con relleno `Gel`. El dominio ya contiene
`MedicamentoLiquido` y SC-1 confirma cosméticos y comestibles, pero el TXT actual
no tiene discriminador ni datos específicos para seleccionar esas variantes.

## Objetivo
política de creación sin modificar `CargadorProductosTxt`.

## Alternativas evaluadas
### 1. Mantener la creación directa
Es la opción más simple, pero mantiene parsing y construcción concreta unidos.

### 2. Agregar if o switch por tipo
Permitiría variantes, pero cada tipo nuevo modificaría nuevamente el cargador.

### 3. Introducir creadores mediante una abstracción
Cada creador encapsula la construcción de un tipo y cumple un contrato común.
Agrega tipos, pero protege al cargador del constructor concreto.

### 4. Utilizar ProductoFactory existente
Se descarta porque `CrearCapsula` fuerza stock mínimo `5` y vencimiento a seis
meses, mientras la carga actual conserva los valores provenientes del TXT.

## Decisión
Adoptar la alternativa 3 e introducir:
- `ICreadorProducto`, con `Producto Crear(DatosProducto datos)`.
- `DatosProducto`, contrato inmutable de datos ya interpretados.
- `CreadorMedicamentoCapsula`, implementación usada para los TXT actuales.

```text
CargadorProductosTxt -> ICreadorProducto
                              ^
                              |
                 CreadorMedicamentoCapsula
```

`CargadorProductosTxt` conservará `File`, delimitador, columnas y conversiones.
Después de interpretar una fila construirá `DatosProducto` y delegará la
creación. La implementación concreta se conectará en el composition root.

## Contrato DatosProducto
Contendrá `Nombre`, `Precio`, `Stock`, `StockMinimo`, `FechaVencimiento` y
`NombreLaboratorio`, con los mismos tipos ya convertidos del modelo actual.

No contendrá `string[]`, índices, delimitadores, `Laboratorio`, enums ni campos
hipotéticos de líquidos, cosméticos o comestibles.

## Comportamiento preservado
`CreadorMedicamentoCapsula` construirá un nuevo laboratorio con el nombre del
TXT, dirección `"Medellin"` y teléfono `"4444444"`. Usará stock mínimo y fecha
recibidos sin recalcularlos y asignará `TipoRelleno.Gel`.

No utilizará `ProductoFactory.CrearCapsula`. Se conservarán orden, mensajes,
duplicados, cultura de conversión y registros previos a una fila fallida.

## Alcance de OCP
Estamos dejando preparado el sistema para que, si en el futuro aparece otro tipo de producto, podamos agregar un nuevo creador sin modificar la lógica de carga. Sin embargo, todavía no podemos cargar varios tipos diferentes desde el mismo TXT porque el archivo actual no indica qué tipo corresponde a cada fila. Esa selección se resolverá cuando SC-1 permita modificar el formato de entrada

## Principios aplicados
- **OCP:** el cargador deja de modificarse por la construcción de una variante.
- **SRP:** parsing y creación quedan en componentes con razones de cambio distintas.
- **LSP:** todo creador retorna un `Producto`; ADR-005 y la matriz LSP del TO-BE
  verificarán la sustituibilidad de los subtipos.

## Consecuencias
Se agregan un contrato, un DTO, un creador y composición explícita. Se acepta la
delegación adicional para aislar la creación y probarla independientemente.

## Fuera de alcance
Discriminador, migración del TXT, creadores líquidos, cosméticos o comestibles,
validaciones nuevas, jerarquía `Producto` y eliminación de `ProductoFactory`.

## Impacto en el UML TO-BE
Se mostrarán `DatosProducto`, `ICreadorProducto`, `CreadorMedicamentoCapsula` y
la dependencia de `CargadorProductosTxt`. Los creadores futuros serán una nota,
no componentes adoptados.

## Criterios de aceptación
1. El cargador no instancia entidades concretas de producto.
2. El creador no lee archivos, divide filas ni convierte textos.
3. Los diez registros actuales continúan siendo `MedicamentoCapsula`.
4. Precio, stock, stock mínimo y fecha conservan los valores del TXT.
5. Laboratorio conserva `"Medellin"` y `"4444444"`; el relleno conserva `Gel`.
6. Mensajes, orden, duplicados y fallas parciales no cambian.
