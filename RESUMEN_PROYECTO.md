# Resumen técnico del proyecto Solución Farmacia

## 1. Descripción general

Solución Farmacia es un prototipo de sistema de gestión farmacéutica ejecutado mediante consola. Está desarrollado en C# sobre .NET 8 y utiliza una biblioteca de clases para representar el dominio y una aplicación de consola para interactuar con el usuario.

Actualmente permite:

- Cargar productos, clientes y usuarios desde archivos TXT.
- Autenticar usuarios mediante nombre de usuario y contraseña.
- Listar y buscar productos.
- Listar clientes.
- Registrar ventas simples.
- Reducir el stock de los productos vendidos.
- Registrar movimientos en memoria.
- Acumular puntos manualmente a los clientes.
- Mostrar alertas por stock mínimo y vencimiento.

El proyecto representa una base académica o demostrativa de programación orientada a objetos. Utiliza conceptos como herencia, interfaces, servicios, factorías y eventos, aunque varias de estas abstracciones todavía no están conectadas con los flujos funcionales principales.

No debe considerarse todavía un sistema farmacéutico o punto de venta completo porque no contiene:

- Base de datos.
- Persistencia de ventas, movimientos, puntos o cambios de inventario.
- Facturación, impuestos, pagos o comprobantes.
- Proveedores, compras o reposición de inventario.
- Control de medicamentos restringidos o recetas.
- Roles y autorizaciones.
- Reportes históricos.
- API, interfaz web o aplicación gráfica.
- Integraciones externas.
- Pruebas automatizadas.

## 2. Tecnologías y estructura de la solución

### 2.1 Tecnologías

- Lenguaje: C#.
- Plataforma: .NET 8.
- Tipo de aplicación: consola.
- Persistencia actual: archivos TXT delimitados por punto y coma.
- Dependencias externas: ninguna.
- Nullable reference types: habilitado.
- Implicit usings: habilitado.

Configuración de los proyectos:

- `BibFarmacia/BibFarmacia.csproj`
- `AppFarmaciaConsola/AppFarmaciaConsola.csproj`

### 2.2 Proyectos

La solución contiene dos proyectos principales.

#### `BibFarmacia`

Biblioteca de clases que contiene:

- Entidades del dominio.
- Servicios.
- Eventos.
- Interfaces.
- Enumeraciones.
- Factoría de productos.
- Ayudantes de autenticación y validación.

#### `AppFarmaciaConsola`

Aplicación ejecutable que contiene:

- Punto de entrada.
- Configuración manual de servicios.
- Carga inicial de archivos.
- Autenticación.
- Menú principal.
- Entrada y salida por consola.
- Parte importante de la lógica de negocio.

La referencia entre ambos proyectos se encuentra en `AppFarmaciaConsola/AppFarmaciaConsola.csproj:10-12`.

## 3. Arquitectura actual

El sistema puede describirse como un monolito modular pequeño ejecutado en un único proceso. Existe una separación inicial entre la aplicación de consola y la biblioteca, pero no se trata de una arquitectura por capas estricta.

Distribución aproximada:

```text
AppFarmaciaConsola
└── Program.cs                 Presentación, configuración y casos de uso

BibFarmacia
├── Clases                     Entidades del dominio
├── Servicios                  Almacenamiento en memoria y operaciones
├── Eventos                    Notificaciones internas
├── Interfaces                 Contratos de descuento y notificación
├── Factories                  Construcción de productos
├── Aspectos                   Autenticación y validación
└── Enums                      Tipos cerrados del dominio
```

### 3.1 Limitaciones arquitectónicas

La separación entre presentación, negocio e infraestructura es incompleta:

- `Program.cs` contiene presentación y reglas de negocio.
- Los servicios leen directamente archivos del sistema operativo.
- Algunas clases de la biblioteca escriben directamente en `Console`.
- Las entidades tienen propiedades completamente mutables.
- Los servicios exponen sus listas internas.
- No existen repositorios ni servicios de aplicación independientes.
- Las dependencias se crean manualmente mediante clases concretas.

Ejemplos:

- Venta y modificación de stock: `AppFarmaciaConsola/Program.cs:255-303`.
- Lectura de productos: `BibFarmacia/Servicios/ServicioProducto.cs:75-118`.
- Presentación desde el dominio: `BibFarmacia/Clases/Producto.cs:29-34`.
- Presentación desde un servicio: `BibFarmacia/Servicios/ServicioNotificacion.cs:10-15`.

## 4. Entidades principales

### 4.1 `Persona`

Clase abstracta con los datos comunes de una persona:

- Nombre.
- Cédula.
- Teléfono.
- Correo electrónico.

Es la clase base de `Cliente` y `Usuario`.

Referencia: `BibFarmacia/Clases/Persona.cs:9-23`.

### 4.2 `Cliente`

Representa una persona que puede acumular puntos.

Características:

- Hereda los datos generales de `Persona`.
- Contiene la propiedad `Puntos`.
- Inicializa los puntos en cero.
- Incluye un método para acumular puntos.

Referencia: `BibFarmacia/Clases/Cliente.cs:9-23`.

Existe una duplicación de responsabilidades porque `Cliente.AcumularPuntos` puede modificar los puntos, pero `ServicioCliente` también modifica directamente la propiedad.

Referencia: `BibFarmacia/Servicios/ServicioCliente.cs:36-45`.

### 4.3 `Usuario`

Representa una persona con acceso al sistema.

Agrega:

- Nombre de usuario.
- Contraseña.

Referencia: `BibFarmacia/Clases/Usuario.cs:8-20`.

No tiene rol, permisos, estado de bloqueo ni información de auditoría. Los nombres Administrador, Supervisor o Vendedor presentes en los datos son únicamente nombres de personas, no roles funcionales.

### 4.4 `Producto`

Clase abstracta que representa un producto del inventario.

Contiene:

- Nombre.
- Precio.
- Stock actual.
- Stock mínimo.
- Fecha de vencimiento.

Referencia: `BibFarmacia/Clases/Producto.cs:8-34`.

Todas las propiedades tienen setters públicos, por lo que cualquier consumidor puede modificar el precio, stock, nombre o vencimiento sin validaciones.

### 4.5 `Medicamento`

Especialización de `Producto` que agrega una asociación con `Laboratorio`.

Referencia: `BibFarmacia/Clases/Medicamento.cs:9-23`.

### 4.6 `MedicamentoCapsula`

Especialización de `Medicamento` que agrega un tipo de relleno:

- Gel.
- Polvo.

Referencias:

- `BibFarmacia/Clases/MedicamentoCapsula.cs:11-28`.
- `BibFarmacia/Enums/TipoRelleno.cs:7-13`.

### 4.7 `MedicamentoLiquido`

Especialización de `Medicamento` que agrega:

- Material del envase.
- Cantidad en mililitros.

Referencias:

- `BibFarmacia/Clases/MedicamentoLiquido.cs:11-31`.
- `BibFarmacia/Enums/MaterialEnvase.cs:7-13`.

Esta entidad existe en el modelo, pero no participa en la carga de datos ni en los flujos actuales.

### 4.8 `Laboratorio`

Representa al fabricante del medicamento.

Contiene:

- Nombre.
- Dirección.
- Teléfono.

Referencia: `BibFarmacia/Clases/Laboratorio.cs:9-22`.

No existe un catálogo de laboratorios ni un identificador compartido. Durante la carga se crea un laboratorio nuevo por cada producto, incluso cuando varios productos podrían pertenecer al mismo fabricante.

### 4.9 `Movimiento`

Representa una operación realizada sobre un producto.

Contiene:

- Fecha.
- Cantidad.
- Tipo de movimiento como texto.
- Referencia al producto.

Referencia: `BibFarmacia/Clases/Movimiento.cs:9-25`.

No registra:

- Identificador del movimiento.
- Cliente relacionado.
- Usuario responsable.
- Precio unitario histórico.
- Total de la operación.
- Stock anterior y posterior.
- Motivo o referencia externa.
- Estado de la operación.

La referencia al producto es mutable. Si el producto cambia después, el movimiento no conserva necesariamente una representación histórica confiable.

## 5. Relaciones del dominio

```text
Persona
├── Cliente
└── Usuario

Producto
└── Medicamento
    ├── MedicamentoCapsula
    └── MedicamentoLiquido

Medicamento ── pertenece a ──> Laboratorio
Movimiento  ── afecta a ─────> Producto
```

Relaciones importantes que faltan:

- Venta con cliente.
- Venta con usuario responsable.
- Venta con uno o varios detalles.
- Movimiento con usuario y motivo.
- Cliente con historial de puntos.
- Laboratorio con identidad propia.
- Producto con código o identificador único.

## 6. Servicios principales

### 6.1 `ServicioProducto`

Responsabilidades actuales:

- Mantener productos en una lista.
- Devolver la lista de productos.
- Cargar productos desde un archivo.
- Interpretar valores textuales.
- Crear medicamentos y laboratorios.
- Detectar stock mínimo.
- Detectar vencimientos.
- Emitir eventos.
- Gestionar errores de carga.

Referencias:

- Colección interna: `BibFarmacia/Servicios/ServicioProducto.cs:14-25`.
- Alertas: `BibFarmacia/Servicios/ServicioProducto.cs:47-73`.
- Carga: `BibFarmacia/Servicios/ServicioProducto.cs:75-118`.

### 6.2 `ServicioCliente`

Responsabilidades actuales:

- Mantener clientes en memoria.
- Cargar clientes desde un archivo.
- Buscar clientes.
- Modificar puntos.
- Emitir eventos de puntos.

Referencia: `BibFarmacia/Servicios/ServicioCliente.cs`.

### 6.3 `ServicioUsuario`

Responsabilidades actuales:

- Mantener usuarios en memoria.
- Cargar usuarios desde un archivo.
- Delegar la validación de credenciales.

Referencia: `BibFarmacia/Servicios/ServicioUsuario.cs`.

### 6.4 `ServicioMovimiento`

Responsabilidades actuales:

- Mantener movimientos en memoria.
- Registrar movimientos.
- Emitir un evento textual.

Referencia: `BibFarmacia/Servicios/ServicioMovimiento.cs:13-38`.

No persiste los movimientos y no garantiza atomicidad con el cambio de stock.

## 7. Flujos funcionales

### 7.1 Inicialización

`Program.cs` instancia directamente los servicios y conecta los manejadores de eventos.

Referencias:

- Creación de servicios: `AppFarmaciaConsola/Program.cs:8-18`.
- Registro de eventos: `AppFarmaciaConsola/Program.cs:20-65`.

Los eventos transportan mensajes ya formateados como texto, lo que los acopla a una presentación humana y dificulta reutilizarlos para auditoría, métricas o integraciones.

### 7.2 Carga de datos

La aplicación carga tres archivos antes de iniciar sesión:

- `productos.txt`.
- `clientes.txt`.
- `usuarios.txt`.

Referencia: `AppFarmaciaConsola/Program.cs:67-89`.

Formatos efectivos:

```text
productos.txt
nombre;precio;stock;stockMinimo;fechaVencimiento;laboratorio

clientes.txt
nombre;cedula;telefono;correo

usuarios.txt
nombre;cedula;telefono;correo;username;password
```

Todos los productos son cargados como `MedicamentoCapsula` con relleno `Gel`, independientemente de su naturaleza.

Referencia: `BibFarmacia/Servicios/ServicioProducto.cs:99-109`.

### 7.3 Autenticación

La consola solicita usuario y contraseña mediante `Console.ReadLine`.

Referencia: `AppFarmaciaConsola/Program.cs:91-124`.

La autenticación consiste en buscar una coincidencia exacta en memoria.

Referencia: `BibFarmacia/Aspectos/AspectoAutenticacion.cs:13-21`.

Todos los usuarios autenticados reciben las mismas opciones porque no existe autorización por rol.

### 7.4 Consulta y búsqueda

La búsqueda:

- Convierte los textos a minúsculas.
- Utiliza coincidencia parcial.
- Retorna el primer resultado.
- No diferencia resultados ambiguos.
- Permite que una cadena vacía coincida con el primer elemento.

Referencias:

- Consulta y búsqueda: `AppFarmaciaConsola/Program.cs:171-253`.
- Búsqueda para venta: `AppFarmaciaConsola/Program.cs:255-270`.

### 7.5 Venta

El flujo actual:

1. Solicita un nombre de producto.
2. Busca una coincidencia parcial.
3. Solicita una cantidad.
4. Resta la cantidad directamente del stock.
5. Crea un movimiento de tipo `Venta`.
6. Registra el movimiento en memoria.

Referencia: `AppFarmaciaConsola/Program.cs:255-303`.

No calcula total, impuestos, descuentos, pagos ni puntos. Tampoco asocia la operación con cliente o usuario.

### 7.6 Puntos

El sistema permite seleccionar un cliente y sumar una cantidad arbitraria de puntos.

Referencia: `AppFarmaciaConsola/Program.cs:305-341`.

No existe:

- Fórmula de acumulación.
- Relación con una venta.
- Historial de puntos.
- Canje de puntos.
- Validación completa de cantidades negativas.
- Persistencia.

### 7.7 Alertas

Después del inicio de sesión se revisa:

- Stock menor o igual al mínimo.
- Vencimiento dentro de 30 días.

Referencias:

- Ejecución de alertas: `AppFarmaciaConsola/Program.cs:135-140`.
- Regla de stock: `BibFarmacia/Servicios/ServicioProducto.cs:47-57`.
- Regla de vencimiento: `BibFarmacia/Servicios/ServicioProducto.cs:59-73`.

La condición de vencimiento también incluye productos que ya vencieron. Por ello, un producto vencido puede mostrarse incorrectamente como próximo a vencer.

## 8. Persistencia y estado

No existe una base de datos. Los archivos TXT se copian al directorio de salida mediante la configuración del proyecto:

`AppFarmaciaConsola/AppFarmaciaConsola.csproj:14-24`.

Los servicios almacenan el estado en listas privadas de memoria. Como resultado:

- El stock modificado se pierde al cerrar.
- Los puntos acumulados se pierden al cerrar.
- Los movimientos se pierden al cerrar.
- No existe historial confiable.
- No hay transacciones.
- No hay control de concurrencia.
- Una carga repetida podría duplicar información.

## 9. Componentes no integrados

Existen componentes que no participan en los flujos actuales:

- `AspectoValidacion`.
- `ServicioDescuento`.
- `ServicioNotificacion`.
- `ProductoFactory`.
- `Cliente.AcumularPuntos`.
- `Producto.MostrarInformacion`.
- `MedicamentoLiquido`.

Referencias:

- `BibFarmacia/Aspectos/AspectoValidacion.cs`.
- `BibFarmacia/Servicios/ServicioDescuento.cs`.
- `BibFarmacia/Servicios/ServicioNotificacion.cs`.
- `BibFarmacia/Factories/ProductoFactory.cs`.

Esto representa deuda técnica porque da la impresión de una arquitectura extensible, aunque el flujo principal no utiliza esas extensiones.

## 10. Partes donde un cambio sería costoso

### 10.1 Migración a base de datos

**Costo estimado: alto.**

Los archivos TXT funcionan como un esquema posicional implícito. La lectura, conversión, construcción de entidades y almacenamiento están mezclados dentro de los servicios.

Una migración requeriría:

- Definir identificadores.
- Definir relaciones y restricciones.
- Crear repositorios.
- Separar persistencia y negocio.
- Migrar datos existentes.
- Persistir stock, puntos y movimientos.
- Incorporar transacciones.
- Decidir reglas de concurrencia.

Principios afectados: SRP y DIP.

### 10.2 Facturación y venta completa

**Costo estimado: alto.**

La venta se encuentra dentro de `Program.cs` y no existe una entidad `Venta`.

Agregar facturación requeriría incorporar:

- Cabecera de venta.
- Detalles de venta.
- Cliente.
- Usuario responsable.
- Precio histórico.
- Descuentos.
- Impuestos.
- Medio de pago.
- Total.
- Devoluciones y anulaciones.
- Persistencia transaccional.

Principios afectados: SRP, OCP y DIP.

### 10.3 Seguridad y roles

**Costo estimado: alto y sensible.**

Las credenciales están en texto plano y no existe autorización.

Una implementación adecuada requeriría:

- Hash de contraseñas.
- Migración de usuarios.
- Roles y permisos.
- Protección de cada caso de uso.
- Cambio y recuperación de contraseña.
- Bloqueo por intentos.
- Auditoría.
- Manejo seguro de datos personales.

Principios afectados: SRP, OCP y DIP.

### 10.4 Nuevos tipos de producto

**Costo estimado: medio-alto.**

Aunque existe una jerarquía, el parser crea siempre cápsulas. Agregar tabletas, cremas, jarabes o inyectables requeriría modificar el formato TXT, el cargador, la factoría, la presentación y las validaciones.

Principio afectado: OCP.

### 10.5 Reportes y auditoría

**Costo estimado: alto.**

Los movimientos actuales no conservan suficientes datos históricos. No es posible producir reportes financieros o de trazabilidad confiables con el modelo actual.

Sería necesario rediseñar `Movimiento` o introducir entidades específicas como `Venta`, `DetalleVenta` y `MovimientoInventario`.

Principios afectados: SRP y OCP.

### 10.6 Cambio de consola a API, web o escritorio

**Costo estimado: medio-alto.**

Las reglas de negocio están mezcladas con `Console` y `Program.cs`. Una nueva interfaz obligaría a extraer los casos de uso o duplicar lógica.

Principios afectados: SRP y DIP.

### 10.7 Reglas de puntos y alertas

**Costo estimado: medio.**

La modificación de puntos está duplicada entre la entidad y el servicio. Las alertas utilizan valores codificados y dependen directamente de `DateTime.Now`.

Un cambio en las reglas podría quedar implementado en un flujo y omitido en otro.

Principios afectados: SRP, OCP y DIP.

## 11. Análisis SOLID

### 11.1 Principio de Responsabilidad Única, SRP

Una clase debería tener un solo motivo para cambiar.

#### Problema en `Program.cs`

`Program.cs` concentra:

- Configuración.
- Presentación.
- Autenticación.
- Navegación.
- Validación de entradas.
- Búsqueda.
- Venta.
- Inventario.
- Puntos.
- Alertas.

Puede cambiar por razones completamente distintas: una nueva interfaz, una regla de venta, una validación o un cambio de seguridad.

Referencia principal: `AppFarmaciaConsola/Program.cs`.

#### Problema en los servicios

Los servicios combinan persistencia, parsing, creación de entidades, almacenamiento y reglas de negocio.

Ejemplo: `BibFarmacia/Servicios/ServicioProducto.cs:47-118`.

#### Consecuencia

Los cambios tienen una superficie amplia, aumentan el riesgo de regresión y dificultan las pruebas unitarias.

### 11.2 Principio Abierto/Cerrado, OCP

El código debería poder extenderse sin modificar componentes estables.

#### Problema en productos

El cargador conoce el tipo concreto que debe construir y siempre crea cápsulas. La factoría también contiene decisiones internas basadas en tipos y valores codificados.

Referencias:

- `BibFarmacia/Servicios/ServicioProducto.cs:99-109`.
- `BibFarmacia/Factories/ProductoFactory.cs:11-43`.

Cada nuevo tipo de producto requiere modificar código existente.

#### Problema en ventas

Las reglas están escritas secuencialmente en `Program.cs`. Agregar descuentos, impuestos o promociones implica modificar el mismo flujo.

#### Problema en eventos

Los eventos emiten mensajes textuales ya formateados. Agregar otros consumidores puede obligar a cambiar el emisor para incluir nueva información.

#### Consecuencia

La extensión del sistema aumenta la cantidad de condicionales y modificaciones distribuidas.

### 11.3 Principio de Sustitución de Liskov, LSP

Los subtipos deberían poder sustituir al tipo base sin alterar el comportamiento esperado.

No se observa una violación directa grave porque los subtipos de `Medicamento` no sobrescriben reglas incompatibles. Sin embargo, la jerarquía está infrautilizada:

- Todos los productos cargados se convierten en cápsulas.
- `MedicamentoLiquido` no participa en flujos reales.
- No existen operaciones polimórficas relevantes.
- Las diferencias dependen principalmente de la construcción externa.

El riesgo aparecerá si se agregan comportamientos específicos que requieran comprobaciones continuas del subtipo o que contradigan las reglas de `Producto`.

#### Consecuencia

El costo actual por LSP es bajo, pero puede crecer si la jerarquía se amplía sin definir contratos claros.

### 11.4 Principio de Segregación de Interfaces, ISP

Los consumidores no deberían depender de métodos que no necesitan.

Las interfaces actuales, `IDescuento` e `INotificacion`, son pequeñas, lo cual es positivo. Sin embargo, no están integradas en los flujos importantes.

Faltan abstracciones en límites relevantes:

- Repositorio de productos.
- Repositorio de clientes.
- Repositorio de usuarios.
- Repositorio de movimientos.
- Servicio de ventas.
- Servicio de autenticación.
- Proveedor de fecha y hora.
- Entrada y salida de usuario.

#### Consecuencia

El problema principal no es que existan interfaces demasiado grandes, sino que las abstracciones actuales no reducen el acoplamiento real.

### 11.5 Principio de Inversión de Dependencias, DIP

Los módulos de alto nivel deberían depender de abstracciones y no de detalles concretos.

Este es uno de los puntos más débiles del proyecto.

#### Dependencias concretas

- `Program.cs` instancia servicios concretos.
- Los servicios dependen directamente de `File`.
- El dominio y servicios dependen de `Console`.
- Las alertas dependen de `DateTime.Now`.
- El almacenamiento depende directamente de `List<T>`.

Referencias:

- `AppFarmaciaConsola/Program.cs:8-18`.
- `BibFarmacia/Servicios/ServicioProducto.cs:75-118`.
- `BibFarmacia/Clases/Producto.cs:29-34`.
- `BibFarmacia/Servicios/ServicioNotificacion.cs:10-15`.
- `BibFarmacia/Servicios/ServicioProducto.cs:59-73`.

#### Consecuencia

Cambiar persistencia, interfaz, reloj o mecanismos de notificación obliga a modificar la lógica existente. También dificulta reemplazar componentes durante las pruebas.

## 12. Matriz SOLID y costo de cambio

| Área | SRP | OCP | LSP | ISP | DIP | Costo |
|---|---|---|---|---|---|---|
| `Program.cs` | Alto | Medio | Bajo | Medio | Alto | Alto |
| Carga de archivos | Alto | Alto | Bajo | Medio | Alto | Alto |
| Venta e inventario | Alto | Alto | Bajo | Medio | Alto | Alto |
| Tipos de producto | Medio | Alto | Medio | Medio | Medio | Medio-alto |
| Autenticación | Alto | Alto | Bajo | Medio | Alto | Alto |
| Puntos | Medio | Medio | Bajo | Bajo | Medio | Medio |
| Alertas | Medio | Medio | Bajo | Bajo | Alto | Medio |
| Reportes y auditoría | Alto | Alto | Bajo | Medio | Alto | Alto |
| Nueva interfaz | Alto | Medio | Bajo | Medio | Alto | Medio-alto |

## 13. Riesgos funcionales y técnicos

### 13.1 Riesgos altos

#### Stock inválido

La venta no comprueba de forma completa que la cantidad sea positiva y menor o igual al stock.

Referencia: `AppFarmaciaConsola/Program.cs:273-292`.

Consecuencias:

- Una cantidad mayor al stock puede dejar inventario negativo.
- Una cantidad negativa puede aumentar el inventario.
- Puede registrarse un movimiento inválido como venta.

#### Credenciales y datos personales en texto plano

Los archivos contienen contraseñas, cédulas, teléfonos y correos sin protección.

Referencias:

- `AppFarmaciaConsola/usuarios.txt`.
- `AppFarmaciaConsola/clientes.txt`.

La contraseña se captura con `Console.ReadLine`, por lo que es visible durante su ingreso.

#### Pérdida del estado

Todos los cambios operativos desaparecen al finalizar el proceso.

#### Falta de autorización

Cualquier usuario autenticado puede consultar clientes, vender, modificar puntos y revisar inventario.

### 13.2 Riesgos medios

- `int.Parse` puede cerrar el programa ante entradas inválidas.
- La carga no es transaccional.
- Una carga repetida puede duplicar elementos.
- `decimal.Parse` y `DateTime.Parse` dependen de la cultura del proceso.
- La validación disponible no se utiliza.
- Las listas internas son expuestas directamente.
- Las entidades son completamente mutables.
- Las búsquedas son parciales y ambiguas.
- Las alertas no diferencian entre vencido y próximo a vencer.
- Las alertas no se ejecutan automáticamente después de una venta.
- Los servicios capturan `Exception` de manera general.
- Los eventos contienen texto en lugar de datos estructurados.

## 14. Deuda técnica

- Código y componentes no utilizados.
- Responsabilidades mezcladas.
- Falta de configuración externa.
- Valores predeterminados codificados.
- Falta de identificadores en entidades.
- Falta de transacciones.
- Falta de encapsulación del estado.
- Inconsistencias de nombres y namespaces.
- Ausencia de documentación funcional previa.
- Datos de demostración con vencimientos obsoletos.
- Ausencia de pruebas automatizadas.
- Ausencia de integración continua.

## 15. Estado de las pruebas

No se encontraron:

- Proyectos de pruebas.
- Dependencias de xUnit, NUnit o MSTest.
- Uso de `Microsoft.NET.Test.Sdk`.
- Aserciones o mocks.
- Pruebas de integración.
- Flujos de integración continua.

### 15.1 Pruebas prioritarias faltantes

#### Ventas e inventario

- Venta válida.
- Cantidad cero.
- Cantidad negativa.
- Stock insuficiente.
- Producto inexistente.
- Actualización de stock.
- Registro de movimiento.
- Atomicidad entre movimiento y stock.

#### Autenticación

- Credenciales válidas.
- Credenciales inválidas.
- Usuarios duplicados.
- Archivo ausente.
- Archivo malformado.
- Roles y permisos cuando se incorporen.

#### Carga de archivos

- Columnas faltantes.
- Columnas adicionales.
- Líneas vacías.
- Fechas inválidas.
- Precios inválidos.
- Culturas diferentes.
- Cargas repetidas.
- Error en mitad de una carga.

#### Alertas

- Stock inferior al mínimo.
- Stock igual al mínimo.
- Stock superior al mínimo.
- Producto vencido.
- Producto próximo a vencer.
- Producto vigente.
- Límite exacto de 30 días.
- Alertas repetidas.

#### Puntos

- Acumulación válida.
- Cantidad negativa.
- Persistencia.
- Relación con una venta.
- Consistencia entre entidad y servicio.

## 16. Prioridades recomendadas

### Prioridad 1: proteger las operaciones actuales

- Validar cantidades de venta.
- Evitar stock negativo.
- Gestionar entradas no numéricas.
- Diferenciar productos vencidos y próximos a vencer.
- Evitar búsquedas vacías o ambiguas.

### Prioridad 2: extraer casos de uso

- Crear un servicio de ventas.
- Sacar reglas de negocio de `Program.cs`.
- Separar presentación y dominio.
- Centralizar las reglas de puntos.

### Prioridad 3: definir persistencia e identidad

- Agregar identificadores.
- Definir repositorios.
- Incorporar una base de datos.
- Persistir ventas, movimientos, stock y puntos.
- Utilizar transacciones.

### Prioridad 4: mejorar seguridad

- Eliminar contraseñas en texto plano.
- Implementar hashing.
- Definir roles y permisos.
- Proteger operaciones sensibles.
- Incorporar auditoría.

### Prioridad 5: preparar extensibilidad

- Integrar correctamente la factoría de productos.
- Definir un discriminador de tipo de producto.
- Estructurar eventos con datos en lugar de mensajes.
- Introducir abstracciones únicamente en límites donde reduzcan acoplamiento.

### Prioridad 6: agregar pruebas

- Cubrir primero venta, inventario y autenticación.
- Incorporar pruebas de carga de archivos.
- Controlar el reloj para probar vencimientos.
- Agregar pruebas de integración para los flujos principales.

## 17. Conclusión

El proyecto ofrece un modelo inicial comprensible y suficiente para demostrar conceptos de programación orientada a objetos. Su tamaño actual permite operarlo como prototipo, pero la mezcla entre presentación, negocio y persistencia hace que su evolución sea costosa.

Los principios SOLID con mayor impacto son:

1. **DIP:** el sistema depende directamente de archivos, consola, reloj y servicios concretos.
2. **SRP:** `Program.cs` y los servicios tienen múltiples motivos para cambiar.
3. **OCP:** agregar reglas de venta o tipos de producto obliga a modificar código existente.
4. **ISP:** las interfaces existentes son pequeñas, pero faltan abstracciones en los límites importantes.
5. **LSP:** no existe una violación grave actual, aunque la jerarquía de productos está infrautilizada.

Las zonas de mayor costo son persistencia, ventas, seguridad, reportes y cambio de interfaz. Antes de agregar facturación, una API o una base de datos, conviene proteger las operaciones existentes con validaciones y pruebas, extraer los casos de uso de `Program.cs` y definir identidades y límites claros entre dominio e infraestructura.
