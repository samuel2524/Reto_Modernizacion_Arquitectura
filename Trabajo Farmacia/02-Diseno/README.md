# 03-diseno — Diseño de la nueva arquitectura (TO-BE), Fase 3

## Estado

| Entregable | Estado |
|---|---|
| Registros de decisión arquitectónica (ADR), mínimo 5 | **Listo** — `ADR/ADR-001` a `ADR-005` |
| Diagrama UML TO-BE con leyenda de colores (fuente editable + imagen) | **Listo** — `diagrama-to-be/` (.drawio, .svg, .pdf, .png) |
| Argumentación de los cinco principios SOLID | **Listo** — cinco notas en el diagrama, una por principio, más los ADR |
| Justificación de herencias con verificación LSP | **Listo** — nota LSP dentro del diagrama |
| Inversiones de dependencia con composition root | **Listo** — nota DIP en el diagrama (alto nivel, abstracción, bajo nivel, composition root) |

> Los diagramas TO-BE anteriores en PlantUML se retiraron de esta carpeta. Siguen
> disponibles en el historial de git (commit `e3be34e` y anteriores) si hicieran
> falta: `git show e3be34e:"Trabajo Farmacia/03-diseno/UML/UML-TO-BE.puml"`.

## Sobre el diagrama TO-BE

Está construido **sobre el diagrama AS-IS**: cada clase que se conserva ocupa la
misma posición y mantiene los mismos atributos y operaciones que en
`../01-diagnostico/diagrama-as-is/`. Así los dos se pueden comparar lado a lado.

- **Negro**: 23 clases conservadas sin cambios.
- **Color por principio**: 4 clases intervenidas y 12 nuevas.
- Las cuatro reasignaciones de responsabilidad que muestra el diagrama
  (eventos de stock, puntos, creación del movimiento de venta y creación de
  productos) son los ADR-001, 003, 004 y 005 dibujados.

El `.drawio` y el `.svg` se pueden abrir y editar en draw.io. El `.pdf` es solo
imagen: la exportación no incrusta el diagrama en ese formato.

## Lo que falta, según el enunciado (sección 6)

### 1. Diagrama UML TO-BE

- [ ] Notación extendida: visibilidad, tipos, multiplicidades, estereotipos
- [ ] **Fuente editable + imagen** (las dos cosas)
- [ ] Convención de color acordada en clase:
  - [ ] **Negro** lo que se conserva del diseño original
  - [ ] **Color** cada elemento intervenido
  - [ ] **Un color por principio aplicado** (SRP, OCP, LSP, ISP, DIP), con su leyenda

> Esta convención es **distinta** a la de los diagramas de Fase 1 y 2, que
> colorean por capa. Aquí se colorea por principio.

### 2. Argumentación de los cinco principios SOLID

No sirve una lista que diga «aplicamos SRP». Por cada principio:

- [ ] Qué clase se partió y en cuántas
- [ ] Por qué esa frontera y no otra
- [ ] Qué gana el negocio con eso

### 3. Justificación de las herencias

Por cada herencia nueva o conservada:

- [ ] Por qué herencia y no composición
- [ ] Verificación explícita de LSP: precondiciones, postcondiciones, invariantes y excepciones
- [ ] Si una jerarquía existente no pasa la verificación, cómo se reemplaza

### 4. Inversiones de dependencia (componente investigativo)

Por cada inversión:

- [ ] Quién es el módulo de **alto nivel**
- [ ] Quién es el de **bajo nivel**
- [ ] Cuál es la **abstracción** que los desacopla
- [ ] En qué punto se resuelve la construcción de los objetos (**composition root**)

> Punto de partida: `../01-diagnostico/mapa-dependencias/` muestra las seis
> dependencias que hoy violan DIP. Cada una es candidata a una inversión.

## Los cinco ADR ya aceptados

| ADR | Decisión | Punto de dolor |
|---|---|---|
| ADR-001 | Extraer el caso de uso de venta de `Program` | PD-01 — venta mezclada con presentación |
| ADR-002 | Desacoplar la carga desde TXT | PD-02 — acoplamiento de la persistencia |
| ADR-003 | Hacer extensible la creación de tipos de producto | PD-03, SC-1 |
| ADR-004 | Separar gestión de clientes y fidelización | H-03 |
| ADR-005 | Separar administración de productos del monitoreo de alertas | H-05 |

Cada ADR debe contener contexto y evidencia (referencia a un hallazgo),
al menos dos alternativas evaluadas, la decisión, el costo aceptado y los
principios involucrados.
