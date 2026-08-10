# Fase 2 — Línea base de las solicitudes de cambio (SolucionFarmacia)

> **Fase 2 del reto** · Los cambios que vienen
> Fecha: 2026-08-06 · Entrega: `/01-diagnostico` · Complementa `Diagnostico_AS-IS.md` (sección 5)
> Herramienta de IA: GitHub Copilot en VS Code — agente "Arquitectura" (`.github/agents/Arquitectura.agent.md`) con skills SOLID.

**Método:** para cada SC aprobada se mide sobre el código actual: clases nuevas (creadas), clases/archivos modificados (con archivo:línea) y comportamiento observable en riesgo. Esta es la columna "antes" de la métrica del criterio 5.

---

## SC-1 — Cosméticos y productos comestibles (gaseosas, agua, helados, snacks)

| Aspecto | Detalle sobre el código actual |
|---|---|
| Clases nuevas | 2: `Cosmetico` y `Comestible` (subclases de `Producto`; categoría como enum) |
| Clases modificadas | 1 obligatoria: `ServicioProducto` · 1 opcional: `Producto` (campo Categoria) |
| Archivos de código modificados | 1-2: `ServicioProducto.cs` (obligatorio), `Producto.cs` (opcional) |
| Archivos de datos modificados | 1: `productos.txt` (hoy 6 campos, sin discriminador de tipo) |
| Líneas críticas | `ServicioProducto.cs` L90-107 (parseo rígido que **siempre** crea `MedicamentoCapsula`), L93-97 (`Laboratorio` hardcodeado) |
| Comportamiento en riesgo | Carga total del inventario al arranque (Program.cs L77-79); alertas stock/vencimiento (L137-139 → L47-73); venta (L280-281) |

**Conteo: 2 nuevas · 1-2 modificadas · 2 archivos modificados · 2 creados · núcleo del dominio tocado (jerarquía `Producto`).**

## SC-2 — Servicios: inyectología, vendajes, curaciones

| Aspecto | Detalle sobre el código actual |
|---|---|
| Clases nuevas | 1-2: `ServicioFarmacia` + enum `TipoServicio` |
| Clases modificadas | 3-4: `Movimiento` (solo referencia `Producto`, L11-14), `Program`, `ServicioMovimiento`, `ServicioCliente` |
| Archivos de código modificados | 2-3: `Movimiento.cs`, `Program.cs`, `ServicioMovimiento.cs` |
| Líneas críticas | `Program.cs` L255-303 (case 4 venta, `Tipo="Venta"` hardcodeado L283-288); `Movimiento.cs` L11-14; `ServicioMovimiento.cs` L25-32 (mensaje observable del evento) |
| Comportamiento en riesgo | Flujo de venta compartido (stock + puntos); mensaje "Movimiento registrado: {tipo}"; regla de puntos |

**Conteo: 1-2 nuevas · 3-4 modificadas · 2-3 archivos modificados · 1-2 creados · toca el flujo de venta (riesgo de regresión alto).**

## SC-3 — Convenios con entidades: descuentos y crédito

| Aspecto | Detalle sobre el código actual |
|---|---|
| Clases nuevas | 3-4: `Convenio`, `Entidad` (tipo de asociado), `Credito`, `ReglaDescuento` |
| Clases modificadas | 3-4: `Cliente`, `ServicioCliente`, `ServicioDescuento` (reactivar: 10% fijo, código muerto), `Program` |
| Archivos de código modificados | 3-4: `Cliente.cs`, `ServicioCliente.cs`, `ServicioDescuento.cs`, `Program.cs` |
| Archivos de datos modificados | 1: `clientes.txt` (entidad, convenio, cupo de crédito) |
| Líneas críticas | `ServicioDescuento.cs` L13-16 (fijo `precio * 0.10m`, sin consumidores, H-05); `Cliente.cs` L11 (solo Puntos); `Program.cs` L255-341 (venta sin descuento, puntos sin convenios) |
| Comportamiento en riesgo | Precio visible en venta (si se agrega línea de descuento cambia la salida); regla de puntos; menú |

**Conteo: 3-4 nuevas · 3-4 modificadas · 3-4 archivos modificados · 3-4 creados · núcleo + venta + código muerto.**

---

## Resumen comparativo (métrica antes)

| SC | Clases nuevas | Clases modificadas | Archivos código modificados | Archivos datos modificados | ¿Núcleo del dominio? |
|---|---|---|---|---|---|
| SC-1 | 2 | 1-2 | 1-2 | 1 | Sí (jerarquía Producto) |
| SC-2 | 1-2 | 3-4 | 2-3 | 0-1 | Parcial (flujo de venta) |
| SC-3 | 3-4 | 3-4 | 3-4 | 1 | Sí (dominio + venta + código muerto) |

**Lectura:** ninguna SC se implementa hoy "agregando código nuevo": todas exigen modificar `ServicioProducto.cs`, `Program.cs` o ambos → síntoma empírico de violación de OCP.
