# ADR-004 — Separar gestión de clientes y fidelización

- **Estado:** Aceptado | **Prioridad:** Media 

## Contexto
`ServicioCliente.cs:14-45` mantiene la colección, agrega y consulta clientes,
modifica puntos y publica `EventoPuntos`. ADR-002 extrae el acceso a TXT, pero
no separa la gestión de clientes de las reglas de fidelización.

`Cliente.AcumularPuntos` ya representa la transición del saldo, aunque el flujo
actual la evita y escribe directamente `cliente.Puntos += puntos`.

## Objetivo
Separar capacidades que cambian por actores distintos: administración de
clientes y coordinación del programa de puntos.

## Alternativas evaluadas
### 1. Mantener ServicioCliente
No agrega tipos, pero conserva dos razones de cambio y dos caminos para mutar
los puntos.

### 2. Mover puntos y eventos a Cliente
Centraliza comportamiento, pero acopla la entidad al mecanismo y texto de
notificación.

### 3. Crear ServicioFidelizacion
Mantiene la transición en la entidad y coordina el evento fuera del dominio.
Agrega una clase y composición explícita.

### 4. Agregar interfaz o bus de eventos
Se descarta porque no existen implementaciones alternativas ni infraestructura
que justifique esa abstracción.

## Decisión
Adoptar la alternativa 3 con esta frontera:

- `Cliente`: mantiene el saldo y ejecuta `AcumularPuntos(puntos)`.
- `ServicioCliente`: administra la colección y coordina su carga según ADR-002.
- `ServicioFidelizacion`: invoca la entidad y publica `EventoPuntos`.
- `Program`: busca el cliente, convierte la entrada, invoca y presenta.

```text
Program -> ServicioCliente -> Cliente
Program -> ServicioFidelizacion -> Cliente
                              -> EventoPuntos
```

No se creará `IServicioFidelizacion`; solo existe una implementación y no hay
una frontera técnica volátil.

## Comportamiento preservado
- La búsqueda mantiene `ToLower().Contains(...)` y la primera coincidencia.
- `int.Parse` permanece en `Program` y conserva sus errores actuales.
- Se aceptan puntos positivos, cero y negativos, sin límites ni validaciones.
- La suma ocurre antes de publicar el evento.
- Se publica exactamente un evento por invocación completada.
- El mensaje conserva nombre e incremento solicitado, no el saldo final.
- Los suscriptores continúan siendo síncronos.
- Una excepción del suscriptor no revierte los puntos ya modificados.
- Los puntos continúan exclusivamente en memoria.

## Principio aplicado
- **SRP:** gestión de clientes y fidelización quedan en componentes distintos.

La extracción no implementa una política nueva, persistencia, transacciones ni
extensibilidad especulativa.

## Consecuencias
Se agrega `ServicioFidelizacion` y cambia la composición en `Program`. Se elimina
la coordinación de puntos de `ServicioCliente`, modificando una API pública cuyo
único consumidor encontrado es `Program`.

Se acepta este costo para evitar que cambios en premios, eventos o reglas de
puntos afecten la administración de clientes.

## Impacto en el UML TO-BE
Desaparece `ServicioCliente -> EventoPuntos`. Aparecen
`Program -> ServicioFidelizacion`, `ServicioFidelizacion -> Cliente` y
`ServicioFidelizacion -> EventoPuntos`.

## Fuera de alcance
Fórmulas por venta, canje, historial, niveles, persistencia, límites de puntos y
restricción del setter público de `Cliente.Puntos`.

## Criterios de aceptación
1. `ServicioCliente` no conoce `EventoPuntos` ni modifica puntos.
2. `ServicioFidelizacion` no conoce colecciones, TXT, `File` ni `Console`.
3. La suma se realiza mediante `Cliente.AcumularPuntos`.
4. Positivos, cero y negativos conservan el resultado actual.
5. Texto, cantidad, sincronía y orden del evento no cambian.
6. La opción 5 conserva búsqueda, mensajes y errores observables.
