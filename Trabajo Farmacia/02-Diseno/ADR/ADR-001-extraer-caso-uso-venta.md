# ADR-001 — Extraer el caso de uso de venta de Program

- **Estado:** Aceptado | **Prioridad:** Alta | 
- **Punto de dolor:** PD-01  Venta mezclada con presentación

## Contexto

En `Program.cs:255-303`, la opción de venta solicita datos, busca el producto,
modifica directamente el stock, crea un `Movimiento` y solicita su registro.
La presentación conoce así la secuencia completa del caso de uso.

SC-2, venta de servicios, y SC-3, convenios, descuentos y crédito, también
presionarán este flujo. Mantenerlo en `Program` aumentaría su responsabilidad y
obligaría a repetir la coordinación en futuros canales.

## Objetivo

Separar la interacción por consola de la coordinación de la venta mediante un
componente reutilizable que no dependa de `Console`.

```text
Program -> ServicioVenta -> Producto / inventario
                        -> registro de Movimiento
```

`Program` conservará composición, menú, lectura y presentación. Esta decisión
solo extrae la coordinación de venta; no elimina sus demás responsabilidades.

## Alternativas evaluadas

### A. Mantener la venta en Program

No agrega tipos, pero conserva presentación y negocio mezclados.

### B. Extraer un método privado en Program

Mejora la organización local, pero mantiene la venta ligada a la consola y no
permite reutilizarla desde otro canal.

### C. Crear ServicioVenta independiente

Centraliza el caso de uso y permite invocarlo desde consola, web o API. Agrega
una clase y dependencias que deben construirse explícitamente.

## Decisión

Adoptar la alternativa C. `ServicioVenta` será responsable de buscar el producto
con la semántica actual, modificar el stock, capturar la fecha, crear el
movimiento y solicitar su registro. Comunicará el resultado sin imprimirlo.

`Program` leerá y convertirá entradas, invocará el servicio y mostrará el
resultado. También continuará conectando los suscriptores de eventos.

Inicialmente, `ServicioVenta` colaborará con `ServicioProducto` y
`ServicioMovimiento` concretos. `Program` los construirá y conectará como
composition root.

## Comportamiento preservado

- La búsqueda seguirá usando `ToLower().Contains(...)` y el primer resultado.
- La coincidencia continuará siendo parcial, sin resolver ambigüedades.
- Si no hay producto, no se modificará stock ni se registrará movimiento.
- La cantidad continuará convirtiéndose con `int.Parse` desde `Program`.
- No se agregarán validaciones de cantidad, stock suficiente o vencimiento.
- El stock seguirá modificándose mediante `Stock -= cantidad`.
- Se capturará `DateTime.Now` después de modificar el stock.
- El movimiento conservará tipo `"Venta"`, cantidad y referencia al producto.
- El movimiento y su evento ocurrirán antes del mensaje de venta registrada.
- No se verificarán automáticamente alertas después de vender.
- Stock y movimientos continuarán exclusivamente en memoria.

## Principios aplicados

- **SRP:** `Program` deja de coordinar la operación de venta.

## Consecuencias

Se agrega `ServicioVenta` y su composición en `Program`. Se acepta esta
complejidad para centralizar una operación crítica y habilitar otros canales.

La extracción no aporta atomicidad: stock y movimiento continúan cambiándose de
forma secuencial, sin rollback. Tampoco introduce pagos, facturación, cliente, usuario vendedor, persistencia, concurrencia o validaciones.

## Impacto en el UML TO-BE

`Program` deja de depender directamente de `Producto` y de crear `Movimiento` en
la venta. Aparece `Program -> ServicioVenta`, que mantiene las colaboraciones necesarias.

## Criterios de aceptación

1. `Program` no modifica stock ni crea movimientos en la opción de venta.
2. `ServicioVenta` no referencia `Console`.
3. Se preservan búsqueda, fecha, orden de efectos y resultados observables.
4. Los mismos casos producen los mismos cambios de stock y movimientos.
5. Una venta no dispara automáticamente las alertas de inventario.
