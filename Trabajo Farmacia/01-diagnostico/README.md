# 01-diagnostico — Fase 1

Diagnóstico del sistema actual (AS-IS). La **Fase 2** (línea base de las tres
solicitudes de cambio) está en `../02-Cambios Que Vienen/`.

## Contenido

| Ruta | Qué es |
|---|---|
| `Diagnostico_AS-IS.docx` | Documento principal: resumen para el comité, inventario de hallazgos H-01…H-09, refutación R-01 a la herramienta, mapa de dependencias, tres puntos de dolor priorizados y trazabilidad con el enunciado |
| `diagrama-as-is/` | Diagrama UML del código actual en notación extendida: 27 clases con atributos y operaciones reales, 36 relaciones con multiplicidades, 8 notas de hallazgos. Fuente `.drawio` + SVG y PDF |
| `mapa-dependencias/` | El mapa de dependencias como diagrama: qué clase depende de cuáles, alto nivel frente a bajo nivel, y dónde se invierte la relación hoy |
| `resumen-as-is.md` | Resumen de apoyo |

## Qué exige el enunciado en esta carpeta

> *"Diagrama AS-IS (fuente editable + imagen), inventario de hallazgos, mapa de
> dependencias, línea base de las tres solicitudes de cambio."*

- [x] Diagrama AS-IS — fuente editable y imagen
- [x] Inventario de hallazgos trazable a archivo y línea
- [x] Al menos 3 hallazgos propios — hay 5 (H-01 a H-05)
- [x] Al menos 1 sugerencia de la herramienta refutada — R-01
- [x] Mapa de dependencias con niveles y punto de inversión
- [x] Tres puntos de dolor priorizados con criterio explícito
- [x] Línea base de las 3 SC — en `../02-Cambios Que Vienen/`

## Nota sobre los diagramas

Los dos diagramas `.drawio` pasan cuatro comprobaciones automáticas: ninguna
línea cruza una caja, cada conexión sale de un punto propio del perímetro, no hay
líneas montadas unas sobre otras y ninguna ruta queda sin definir.

Si se editan a mano en draw.io, **volver a verificarlos antes de exportar**:
arrastrar el extremo de una flecha la engancha al compartimento interno de la caja
en vez de a la caja, y eso rompe el trazado sin que se note a simple vista.
