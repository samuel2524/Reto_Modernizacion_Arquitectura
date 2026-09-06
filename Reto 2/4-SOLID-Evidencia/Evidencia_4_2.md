# Evidencia 4.2: el comportamiento no cambió

Dos partes. La primera son los 12 casos de caracterización del Reto 1,
corridos contra el TO-BE del Reto 1 (`03-src`, sin patrones nuevos) y contra
el código real del Reto 2 (con los tres patrones adoptados: Strategy,
Composite y Template Method), comparados byte a byte. Factory Method no es
un patrón incorporado en este reto: sigue igual que en el Reto 1, como
parte del AS-IS. La segunda son 4 casos nuevos que ejercitan Strategy, el
único patrón que agrega comportamiento observable nuevo (la venta con
convenio). Composite y Template Method se adoptaron para que la salida
**no** cambiara, así que la prueba de que cumplieron su objetivo son los 12
casos heredados, no un camino nuevo.

## Parte 1: los 12 casos heredados

Mismas teclas que `03-src/ejecutar-caracterizacion.ps1` (documentado en
`04-evidencia/casos-de-caracterizacion/casos-de-caracterizacion.md`), corridas
con `dotnet run --no-build` contra dos binarios: el de `03-src` (TO-BE Reto 1)
y el del código real del Reto 2 (`Reto 2/codigo`, tal como está en el
repositorio del equipo). Los inventarios de productos y usuarios son
idénticos en ambos; `clientes.txt` difiere solo en que Carlos, Ana y Juan
llevan además su columna de convenio. Ningún camino de estos 12 casos
imprime ese dato (ni el listado de clientes, ni la acumulación de puntos),
así que no afecta la comparación.

| Caso | Escenario | Resultado |
|---|---|---|
| CC-01 | Arranque: carga de los tres archivos y alertas iniciales | Idéntica |
| CC-02 | Login con credenciales válidas | Idéntica |
| CC-03 | Login con credenciales inválidas | Idéntica |
| CC-04 | Listar productos | Idéntica |
| CC-05 | Listar clientes con sus puntos | Idéntica |
| CC-06 | Buscar un producto que existe | Idéntica |
| CC-07 | Buscar un producto que no existe | Idéntica |
| CC-08 | Venta con stock suficiente | Idéntica |
| CC-09 | Venta mayor al stock disponible (H-07 se reproduce igual) | Idéntica |
| CC-10 | Acumular puntos a un cliente | Idéntica |
| CC-11 | Ver alertas de stock mínimo y vencimiento (pasa por `MonitorCompuesto`) | Idéntica |
| CC-12 | Opción de menú que no existe | Idéntica |

**Resultado: 12 de 12 casos con salida idéntica.** CC-01 y CC-11 son la prueba
directa de Template Method y Composite: CC-01 ejercita los tres `CargadorTxt<T>`
(antes tres cargadores independientes, ahora una plantilla común) y CC-11
ejercita `MonitorCompuesto` (antes `ServicioMonitoreoProductos`, hoy la clase
ya no existe y `Program` usa el compuesto a través de `IReglaAlerta`); que
ambos den exactamente la misma salida de antes confirma que la reorganización
interna no movió ni una línea de lo que el usuario ve. CC-04, que lista el
inventario completo (medicamentos, cosméticos y comestibles), es la prueba de
Factory Method: `SelectorCreadorProducto` sigue siendo el único mecanismo de
creación y no se tocó.

Archivos completos en `evidencia-4.2/reto1/` y `evidencia-4.2/reto2/`
(`CC-01.txt` a `CC-12.txt`); el resumen comparado está en
`evidencia-4.2/resumen_12.csv`.

## Parte 2: 4 casos nuevos de Strategy (venta con convenio)

La venta con convenio es un flujo aparte (`--convenio` en la línea de
comandos, ver `Program.cs`) que no toca `ServicioVenta`. Los 4 casos cubren
las tres políticas concretas y el rechazo más común. En los tres aprobados
el número se predijo a mano contra `PoliticaSoloDescuento` /
`PoliticaSoloCredito` / `PoliticaDescuentoCredito` antes de correr el
programa, y los tres coincidieron exactos con la salida real.

| Caso | Escenario | Convenio | Cálculo esperado | Resultado real |
|---|---|---|---|---|
| CN-01 | `PoliticaSoloDescuento` aprobada | Carlos, EmpresaAcme, 10 %, cupo 0 | Omeprazol x2 = 30000; descuento 10 % = 3000; total 27000; cupo restante 0 (no se toca en esta política) | Subtotal 30000 / Descuento 3000 / Total 27000 / Cupo restante 0 (coincide) |
| CN-02 | `PoliticaSoloCredito` aprobada | Ana, BancoCentral, cupo 20000 | Dolex x2 = 10000; sin descuento; cupo alcanza: cupo restante 20000 − 10000 = 10000 | Subtotal 10000 / Descuento 0 / Total 10000 / Cupo restante 10000 (coincide) |
| CN-03 | `PoliticaDescuentoCredito` aprobada | Juan, UniversidadCentral, 10 %, cupo 20000 | Acetaminofen x3 = 13500; descuento 10 % = 1350; total 12150; cupo alcanza: cupo restante 20000 − 12150 = 7850 | Subtotal 13500 / Descuento 1350 / Total 12150 / Cupo restante 7850 (coincide) |
| CN-04 | Rechazo: cliente sin convenio | Maria (cédula 741), no tiene convenio | `ServicioVentaConvenio` debe rechazar antes de tocar cualquier política, con `Motivo = "Cliente sin convenio"` | "Venta con convenio rechazada: Cliente sin convenio" (coincide) |

En CN-01, CN-02 y CN-03 el evento `MovimientoRegistrado` se dispara (el
mismo `Observer` de siempre): `ServicioVentaConvenio` sí descuenta stock y
registra el movimiento cuando el resultado viene `Aprobado`. En CN-04 ese
evento no aparece, porque el rechazo corta el flujo antes de tocar
inventario o movimientos. Ese es justo el comportamiento que la
Ficha_Strategy declaraba como pendiente de decidir, y aquí queda
implementado y comprobado.

Salidas completas en `evidencia-4.2/nuevos/` (`CN-01.txt` a `CN-04.txt`).

## Qué prueba esto y qué no

Prueba que la venta normal, las alertas, la carga de archivos y la creación
de productos (todo lo que ya validaban los 12 casos del Reto 1) siguen
produciendo exactamente la misma salida después de meter los cuatro
patrones. Prueba también que el único comportamiento nuevo (convenios) hace
lo que la Ficha_Strategy y la Tabla de cambio estructural dicen que debe
hacer, con los números exactos. No repite lo que la Matriz SOLID ya
sustenta (que ningún principio quedó roto); esa evidencia está en
`Matriz_SOLID.md`.
