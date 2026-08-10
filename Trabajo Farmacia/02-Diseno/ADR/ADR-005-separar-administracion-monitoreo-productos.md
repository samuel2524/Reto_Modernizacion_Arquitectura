# ADR-005 — Separar administración de productos del monitoreo de alertas

- **Estado:** Aceptado | **Prioridad:** Media | **Fecha:** 2026-08-07
- **Relacionado:** H-04, ADR-002, ADR-003 y SC-1

## Contexto
Después de ADR-002 y ADR-003, `ServicioProducto` aún administra la colección,
coordina la carga, evalúa stock y vencimiento y publica alertas. Catálogo y
monitoreo cambian por razones diferentes.

La extracción es estructural: no corrige las reglas actuales ni el uso directo
de `DateTime.Now`.

## Objetivo
Separar la administración de productos de la evaluación y publicación de
condiciones de alerta.

## Alternativas evaluadas
### 1. Mantener ServicioProducto
No agrega tipos, pero conserva catálogo y monitoreo en la misma clase.

### 2. Crear ServicioMonitoreoProductos dependiente de ServicioProducto
Separa métodos, pero acopla el monitor a toda la API de administración.

### 3. Recibir IEnumerable<Producto> en cada verificación
Expresa solo los datos necesarios, facilita pruebas y evita una interfaz propia.

### 4. Crear IConsultaProductos
Se posterga hasta que exista otro consumidor o una frontera compartida que
justifique mantener un contrato adicional.

## Decisión
Adoptar la alternativa 3 y crear `ServicioMonitoreoProductos`.

- `ServicioProducto` conserva colección, alta, consulta y carga.
- `ServicioMonitoreoProductos` evalúa alertas y posee `EventoStockMinimo` y
  `EventoVencimiento`.
- `VerificarStock` y `VerificarVencimiento` reciben `IEnumerable<Producto>`.
- `Program` construye ambos servicios, conecta suscriptores y proporciona los
  productos al ejecutar cada verificación.

```text
Program -> ServicioProducto -> Producto
Program -> ServicioMonitoreoProductos -> IEnumerable<Producto>
                                      -> EventoStockMinimo
                                      -> EventoVencimiento
```

No se creará una interfaz para el monitor porque solo existe una implementación
y no representa una frontera técnica volátil.

## Comportamiento preservado
- Stock alerta cuando `Stock <= StockMinimo`, incluso con valores negativos.
- Se recorre la colección en el orden actual.
- Vencimiento calcula `(FechaVencimiento - DateTime.Now).Days` por producto.
- Se alerta cuando el resultado es `<= 30`, incluyendo productos vencidos.
- `DateTime.Now` permanece dentro del recorrido y conserva la truncación actual.
- Cada invocación publica nuevamente todas las coincidencias, sin deduplicar.
- Se mantienen textos, sincronía, orden y propagación de excepciones.
- La carga inicial y la opción 6 verifican primero stock y luego vencimiento.
- Las ventas no disparan alertas automáticamente.

## Principio aplicado
- **SRP:** catálogo y monitoreo quedan en componentes con razones de cambio
  independientes.

No se afirma DIP completo porque el reloj y las clases concretas de eventos se
mantienen. Su inversión requiere una decisión separada.

## Condición LSP
El monitor opera exclusivamente sobre `Producto`, sin downcasts ni condiciones
por subtipo. Todo producto futuro debe soportar legítimamente stock y vencimiento.

Si un tipo no posee alguna capacidad, no se usarán fechas ficticias, métodos
vacíos ni `NotSupportedException`; deberá revisarse el contrato mediante
composición o interfaces de capacidad y documentarse en la matriz LSP TO-BE.

## Consecuencias
Se agrega una clase y cambia la composición en `Program`. Los eventos conservan
sus contratos, pero su propietario pasa de `ServicioProducto` al monitor.

Se acepta esta complejidad para evitar que cambios en umbrales, repetición o
nuevas alertas afecten la administración del catálogo.

## Impacto en el UML TO-BE
Desaparecen las relaciones de `ServicioProducto` con los eventos. Aparecen
`Program -> ServicioMonitoreoProductos` y las relaciones del monitor con
`Producto`, `EventoStockMinimo` y `EventoVencimiento`.

## Fuera de alcance
Reloj inyectable, deduplicación, persistencia, bus de eventos, nuevas alertas,
validaciones y cambios en la jerarquía de productos.
## Criterios de aceptación
1. `ServicioProducto` no conoce eventos ni reglas de alerta.
2. El monitor no conoce carga, archivos, consola ni `ServicioProducto`.
3. Una colección vacía no publica alertas.
4. Límites de stock y vencimiento conservan los resultados actuales.
5. Vencidos alertan y verificaciones repetidas vuelven a publicar.
6. Mensajes, cantidad, orden y sincronía no cambian.
7. Las mismas pruebas de monitoreo se ejecutan con cada subtipo de `Producto`.
