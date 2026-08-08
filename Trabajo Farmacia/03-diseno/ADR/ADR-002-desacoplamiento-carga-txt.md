# ADR-002 — Desacoplamiento de la carga desde TXT

- **Estado:** Aceptado
- **Fecha:** 2026-08-07
- **Punto de dolor:** PD-02 — Acoplamiento de la persistencia TXT

## Contexto

`ServicioProducto`, `ServicioCliente` y `ServicioUsuario` dependen directamente
de `File`, del separador `;`, de posiciones `datos[x]` y de conversiones propias
del formato TXT. La evidencia completa está registrada en el inventario AS-IS.

Los TXT solo proporcionan la carga inicial. Los cambios de stock, puntos y
movimientos no se escriben posteriormente, por lo que el comportamiento actual
no corresponde a un repositorio CRUD persistente.

## Objetivo

Separar las políticas de los servicios del mecanismo concreto de lectura para
que no conozcan `File`, el delimitador, las columnas ni el parsing del TXT.

## Módulos y dirección de dependencias

- **Alto nivel:** `ServicioProducto`, `ServicioCliente` y `ServicioUsuario`.
- **Bajo nivel:** lectura con `File`, formato TXT, parsing y creación desde filas.
- **Abstracciones:** `ICargadorProductos`, `ICargadorClientes` e `ICargadorUsuarios`.
- **Adaptadores:** `CargadorProductosTxt`, `CargadorClientesTxt` y `CargadorUsuariosTxt`.
- **Composition root:** `Program` construye adaptadores, los inyecta en los servicios y conserva las rutas actuales.

```text
ServicioProducto  -> ICargadorProductos <- CargadorProductosTxt
ServicioCliente   -> ICargadorClientes  <- CargadorClientesTxt
ServicioUsuario   -> ICargadorUsuarios  <- CargadorUsuariosTxt
```

## Alternativas evaluadas

### 1. Mantener la lectura TXT dentro de los servicios

No agrega tipos, pero conserva el acoplamiento y las responsabilidades mezcladas.

### 2. Extraer clases auxiliares concretas sin interfaces

Reduce repetición y mejora SRP, pero los servicios siguen dependiendo de una
implementación y del mecanismo de carga seleccionado.

### 3. Introducir cargadores específicos mediante abstracciones

Separa la política del detalle TXT y permite sustituir la fuente durante pruebas
o ante un cambio confirmado de almacenamiento.

### 4. Crear repositorios CRUD completos

Se descarta porque inventaría escritura, actualización, eliminación, identidad
y consistencia persistente que el sistema actual no ofrece.

## Decisión

Adoptar la alternativa 3. Cada contrato expondrá únicamente la operación de
carga requerida por su consumidor. No tendrá métodos `Guardar`, `Actualizar` o
`Eliminar`. Los adaptadores TXT concentrarán `File`, delimitador, columnas,
conversiones y construcción de entidades.

Los servicios conservarán sus colecciones en memoria. La carga seguirá siendo
explícita antes del inicio de sesión y no se realizará E/S en constructores.

## Principios aplicados

- **DIP:** las políticas dependen de puertos y no de `File` o TXT.
- **ISP:** cada servicio recibe un contrato pequeño para su necesidad concreta.
- **SRP:** lectura, parsing y mapeo quedan en adaptadores especializados.

## Comportamiento preservado

- Los mismos TXT producen los mismos objetos, en el mismo orden.
- Los productos continúan cargándose como `MedicamentoCapsula` con relleno `Gel`.
- Se conservan los valores actuales del laboratorio y los mensajes observables.
- Las cargas repetidas continúan agregando duplicados y una falla conserva las
  líneas válidas cargadas previamente.
- Stock, puntos y movimientos continúan exclusivamente en memoria.

## Consecuencias

Se agregan tres contratos y tres adaptadores, además de composición explícita en
`Program`. Se acepta este costo para reducir acoplamiento y permitir pruebas sin
archivos. La decisión no implementa base de datos ni persistencia de cambios.

## Fuera de alcance

Tipos líquidos, validación del formato, transacciones, seguridad, ventas,
control del reloj y corrección de datos predeterminados pertenecen a otros ADR.

## Criterios de aceptación

1. Los servicios no referencian `File`, delimitadores ni índices de columnas.
2. Los archivos actuales cargan 10 productos, 10 clientes y 5 usuarios.
3. La autenticación y las alertas conservan su comportamiento observable.
4. Un archivo inexistente mantiene el resultado `"Archivo no encontrado"`.
5. Ningún adaptador escribe en los TXT.
6. `Program` contiene la construcción y conexión de adaptadores y servicios.
