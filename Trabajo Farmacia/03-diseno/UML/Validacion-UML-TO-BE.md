# Validación del UML TO-BE

## 1. Alcance y fuentes

Los diagramas representan el diseño aprobado por los ADR-001 a ADR-005 y las
firmas y namespaces aprobados posteriormente. El código C# de producción sigue
en estado AS-IS; estos artefactos son documentación TO-BE y no afirman que la
implementación ya exista.

Fuentes contrastadas:

- Todo el código C# propio de `BibFarmacia` y `AppFarmaciaConsola`.
- Los cinco ADR de `Trabajo Farmacia/03-diseno/ADR`, todos aceptados.
- Los UML AS-IS, para conservar identificadores y miembros existentes.

No se agregan repositorios CRUD, persistencia de cambios, reloj inyectable,
interfaces para venta/fidelización/monitoreo, tipos de producto futuros ni buses
de eventos.

## 2. Archivos y responsabilidad

| Archivo | Contenido |
| --- | --- |
| `UML-TO-BE.puml` | Mapa integrado de tipos y dependencias TO-BE. |
| `UML-TO-BE-01-General.puml` | Inventario por namespace y trazabilidad hacia las vistas detalladas. |
| `UML-TO-BE-02-Casos-Uso-Servicios.puml` | Miembros de servicios, venta, fidelización, eventos y composition root. |
| `UML-TO-BE-03-Carga-Adaptadores.puml` | Firmas de carga, puertos, adaptadores TXT, DTO y creador de cápsulas. |
| `UML-TO-BE-04-Dominio-Monitoreo.puml` | Dominio AS-IS completo, monitoreo, eventos y matriz LSP referenciada. |

## 3. Trazabilidad de decisiones

| ADR | Elementos y cambios representados |
| --- | --- |
| ADR-001 | `ServicioVenta`, `BuscarProducto`, `Vender`, colaboración con `ServicioProducto` y `ServicioMovimiento`; `Program` deja de crear el movimiento de venta. |
| ADR-002 | Tres `ICargador*`, tres cargadores TXT, inyección por constructor y servicios sin dependencia directa de `File` o columnas TXT. |
| ADR-003 | `DatosProducto`, `ICreadorProducto`, `CreadorMedicamentoCapsula` y delegación desde `CargadorProductosTxt`; `ProductoFactory` no participa. |
| ADR-004 | `ServicioFidelizacion`, uso de `Cliente.AcumularPuntos` y traslado de `EventoPuntos` fuera de `ServicioCliente`. |
| ADR-005 | `ServicioMonitoreoProductos`, traslado de eventos y verificaciones fuera de `ServicioProducto`, entrada `IEnumerable<Producto>`. |

### Elementos TO-BE

| Elemento TO-BE | Existía AS-IS | ADR origen | Principio principal | Elemento reemplazado/intervenido | Justificación |
| --- | --- | --- | --- | --- | --- |
| `ServicioVenta` | No | ADR-001 | SRP | Coordinación de venta en `Program` | Separa consola de búsqueda, modificación de stock y registro del movimiento. |
| `ICargadorProductos` | No | ADR-002 | DIP | Dependencia de `ServicioProducto` con `File` y TXT | El servicio depende de la capacidad específica de carga; ISP es secundario. |
| `ICargadorClientes` | No | ADR-002 | DIP | Dependencia de `ServicioCliente` con `File` y TXT | Aísla lectura, columnas y conversiones del formato. |
| `ICargadorUsuarios` | No | ADR-002 | DIP | Dependencia de `ServicioUsuario` con `File` y TXT | Aísla la carga inicial sin inventar un repositorio CRUD. |
| `CargadorProductosTxt` | No | ADR-002/003 | DIP | Parsing dentro de `ServicioProducto` | Conserva el detalle TXT y delega solamente la construcción del producto. |
| `CargadorClientesTxt` | No | ADR-002 | DIP | Parsing dentro de `ServicioCliente` | Materializa el contrato con el formato y comportamiento AS-IS. |
| `CargadorUsuariosTxt` | No | ADR-002 | DIP | Parsing dentro de `ServicioUsuario` | Materializa el contrato con el formato y comportamiento AS-IS. |
| `ICreadorProducto` | No | ADR-003 | OCP | Construcción concreta desde el cargador | Permite variar la construcción sin modificar el parsing. |
| `DatosProducto` | No | ADR-003 | OCP | Paso de columnas físicas al creador | Transporta únicamente valores ya interpretados y aprobados. |
| `CreadorMedicamentoCapsula` | No | ADR-003 | OCP | `new MedicamentoCapsula` en la carga | Preserva laboratorio, relleno, stock mínimo y vencimiento actuales. |
| `ServicioFidelizacion` | No | ADR-004 | SRP | Puntos y `EventoPuntos` en `ServicioCliente` | Separa gestión de clientes de coordinación de fidelización. |
| `ServicioMonitoreoProductos` | No | ADR-005 | SRP | Alertas y eventos en `ServicioProducto` | Separa catálogo de monitoreo y recibe productos por operación. |
| `ServicioProducto` intervenido | Sí | ADR-002/005 | DIP | Lectura TXT y monitoreo internos | Conserva colección, alta, consulta y coordinación de carga. |
| `ServicioCliente` intervenido | Sí | ADR-002/004 | DIP | Lectura TXT y coordinación de puntos | Conserva colección, alta, consulta y coordinación de carga. |
| `ServicioUsuario` intervenido | Sí | ADR-002 | DIP | Lectura TXT interna | Conserva usuarios, autenticación y coordinación de carga. |

### Dependencias invertidas por ADR-002

| Dependencia | Alto nivel | Abstracción | Bajo nivel | Composition Root | ADR |
| --- | --- | --- | --- | --- | --- |
| Carga de productos | `ServicioProducto` | `ICargadorProductos` | `CargadorProductosTxt`, `File`, parsing TXT y columnas | `Program` crea el adaptador y lo inyecta | ADR-002 |
| Carga de clientes | `ServicioCliente` | `ICargadorClientes` | `CargadorClientesTxt`, `File`, parsing TXT y columnas | `Program` crea el adaptador y lo inyecta | ADR-002 |
| Carga de usuarios | `ServicioUsuario` | `ICargadorUsuarios` | `CargadorUsuariosTxt`, `File`, parsing TXT y columnas | `Program` crea el adaptador y lo inyecta | ADR-002 |
| Construcción del producto cargado | `CargadorProductosTxt` | `ICreadorProducto` | `CreadorMedicamentoCapsula` | `Program` crea el creador y lo inyecta al adaptador | ADR-003 |

## 4. Firmas aprobadas y modeladas

```text
ICargadorProductos.Cargar(ruta: string,
  destino: ICollection<Producto>): string
ICargadorClientes.Cargar(ruta: string,
  destino: ICollection<Cliente>): string
ICargadorUsuarios.Cargar(ruta: string,
  destino: ICollection<Usuario>): string

ServicioVenta.BuscarProducto(nombre: string): Producto?
ServicioVenta.Vender(producto: Producto, cantidad: int): string

ICreadorProducto.Crear(datos: DatosProducto): Producto
ServicioFidelizacion.AcumularPuntos(cliente: Cliente, puntos: int): void
ServicioMonitoreoProductos.VerificarStock(
  productos: IEnumerable<Producto>): void
ServicioMonitoreoProductos.VerificarVencimiento(
  productos: IEnumerable<Producto>): void
```

Identificadores de eventos conservados:

```text
ServicioFidelizacion.EventoPuntos: EventoPuntos
ServicioMonitoreoProductos.EventoStock: EventoStockMinimo
ServicioMonitoreoProductos.EventoVencimiento: EventoVencimiento
```

Los métodos públicos de carga de los tres servicios conservan sus nombres y
retornos AS-IS; delegan en el contrato inyectado y entregan su colección como
`ICollection<T>`. Esto preserva mensajes, orden, duplicados y registros válidos
agregados antes de una fila fallida.

## 5. Namespaces

| Namespace | Tipos TO-BE nuevos |
| --- | --- |
| `BibFarmacia.Interfaces` | `ICargadorProductos`, `ICargadorClientes`, `ICargadorUsuarios`, `ICreadorProducto` |
| `BibFarmacia.Servicios` | `ServicioVenta`, `ServicioFidelizacion`, `ServicioMonitoreoProductos`, `CargadorProductosTxt`, `CargadorClientesTxt`, `CargadorUsuariosTxt` |
| `BibFarmacia.Clases` | `DatosProducto` |
| `BibFarmacia.Factories` | `CreadorMedicamentoCapsula` |

`Program` permanece en el espacio de nombres global como programa de nivel
superior y actúa como composition root.

## 6. Justificación de relaciones y multiplicidades

| Relación | Multiplicidad | Justificación |
| --- | --- | --- |
| Servicio de carga — `ICargador*` | `1` | Dependencia obligatoria recibida por constructor. |
| `CargadorProductosTxt` — `ICreadorProducto` | `1` | Creador obligatorio recibido por constructor. |
| Servicio — colección de entidad | `0..*` | Las listas empiezan vacías y permanecen en memoria. |
| `ServicioVenta` — servicios colaboradores | `1` y `1` | Ambos son necesarios para buscar/modificar producto y registrar movimiento. |
| Nuevo servicio — evento que posee | `1` | Cada servicio crea y expone exactamente la instancia identificada por el ADR. |
| `Medicamento` — `Laboratorio` | `1` | Propiedad y constructor AS-IS no anulables. |
| `Movimiento` — `Producto` | `1` | Propiedad y constructor AS-IS no anulables. |
| Monitor/cargadores — entidades recibidas | Dependencia | Uso por parámetro; no implica propiedad adicional. |
| `Program` — componentes concretos | Dependencia `crea` | Composition root; construye e interconecta implementaciones. |

No se modelan multiplicidades de archivos, filas o suscriptores porque no hay
campos propios ni contratos aprobados que las definan.

## 7. Matriz LSP verificable

La jerarquía se conserva gris. El naranja se reserva para la nota compacta del
diagrama UML-04 y no identifica clases nuevas o modificadas.

| Contrato observado a través de `Producto` | `Medicamento` | `MedicamentoCapsula` | `MedicamentoLiquido` | Resultado exigido |
| --- | --- | --- | --- | --- |
| Leer y modificar `Stock` | Verificable | Verificable | Verificable | Misma transición, sin downcast. |
| Comparar `Stock <= StockMinimo` | Verificable | Verificable | Verificable | Publicar una alerta por coincidencia y por invocación. |
| Calcular días desde `FechaVencimiento` | Verificable | Verificable | Verificable | Mantener `DateTime.Now`, truncación y límite `<= 30`. |
| Producto vencido | Verificable | Verificable | Verificable | También publica alerta. |
| Colección vacía | Verificable | Verificable | Verificable | No publica alertas. |
| Orden de recorrido | Verificable | Verificable | Verificable | Igual al orden de la colección. |
| Verificación repetida | Verificable | Verificable | Verificable | Vuelve a publicar; no deduplica. |
| Venta mediante miembros de `Producto` | Verificable | Verificable | Verificable | Resta cantidad y registra `Movimiento` sin condición por subtipo. |
| `MostrarInformacion()` heredado | Verificable | Verificable | Verificable | Ningún subtipo cambia el contrato AS-IS. |

También puede comprobarse estructuralmente que `Cliente` y `Usuario` conservan
el contrato de datos heredado de `Persona`; ninguno sobrescribe comportamiento
base ni fortalece precondiciones.

No se afirma LSP para cosméticos, comestibles, productos sin stock/vencimiento o
creadores futuros: esos tipos no forman parte de las decisiones adoptadas.

## 8. Decisiones NO implementadas

Deliberadamente no se introducen:

- Base de datos, persistencia de cambios o repositorios CRUD.
- Cargadores JSON, API u otros adaptadores reales.
- Escritura, actualización o eliminación en los archivos TXT.
- `SelectorCreadorProducto` o `ISelectorCreadorProducto`.
- Creadores de medicamentos líquidos, cosméticos o comestibles.
- Discriminadores o columnas nuevas en el TXT actual.
- Nuevos tipos de productos o cambios en la jerarquía existente.
- Validación de stock suficiente o cantidad positiva.
- Prohibición de vender productos vencidos.
- Nuevas reglas de descuentos, fidelización o alertas.
- Reloj inyectable; se conserva `DateTime.Now`.
- Deduplicación, alertas automáticas posteriores a una venta o asincronía.
- Bus de eventos, transacciones, rollback o control de concurrencia.
- Interfaces para `ServicioVenta`, `ServicioFidelizacion` o
  `ServicioMonitoreoProductos`.

`ProductoFactory` tampoco se elimina: permanece como elemento AS-IS conservado,
pero no participa en el flujo de creación aprobado por ADR-003.

## 9. Conservación de comportamiento

- `ServicioVenta.BuscarProducto` mantiene `ToLower().Contains(...)` y el primer
  resultado. La separación en dos operaciones permite que `Program` solicite y
  convierta la cantidad solo después de encontrar el producto.
- `Vender` conserva resta directa de stock, `DateTime.Now`, tipo `"Venta"`,
  registro del movimiento antes del resultado `"Venta registrada"` y ausencia
  de alertas automáticas.
- Los cargadores mantienen mensajes AS-IS, cultura de conversión, orden,
  duplicados y carga parcial anterior a una fila fallida.
- `CreadorMedicamentoCapsula` usa los valores interpretados, laboratorio con
  dirección `"Medellin"` y teléfono `"4444444"`, y `TipoRelleno.Gel`.
- Fidelización acepta puntos positivos, cero y negativos; modifica mediante
  `Cliente.AcumularPuntos` antes de disparar exactamente un evento síncrono.
- Monitoreo conserva límites, orden, repetición, textos, sincronía, reloj real y
  propagación de excepciones.

## 10. Validación visual y sintáctica

Los cinco `.puml` se procesaron correctamente con PlantUML 1.2025.4 y se
generaron sus PNG a 180 DPI, sin errores de sintaxis:

| Archivo PNG | Dimensiones |
| --- | --- |
| `UML-TO-BE.png` | 2300 × 3834 px |
| `UML-TO-BE-01-General.png` | 7504 × 1477 px |
| `UML-TO-BE-02-Casos-Uso-Servicios.png` | 3472 × 2814 px |
| `UML-TO-BE-03-Carga-Adaptadores.png` | 2028 × 2823 px |
| `UML-TO-BE-04-Dominio-Monitoreo.png` | 5250 × 1880 px |

La inspección a resolución completa confirmó que las clases y leyendas no están
cortadas. La vista integrada prioriza comprensión global; las vistas 02 a 04
distribuyen firmas y relaciones para evitar que el detalle vuelva ilegible el
mapa general.

### Lista de comprobación final

| Criterio | Resultado | Evidencia |
| --- | --- | --- |
| Cada clase nueva proviene de un ADR aprobado | Cumple | Trazabilidad de la sección 3 y estereotipos `ADR-*`. |
| No se inventaron reglas de negocio | Cumple | Sección 9 y decisiones excluidas de la sección 8. |
| `ServicioProducto` ya no conoce `File`/TXT | Cumple | Depende de `ICargadorProductos`; el detalle está en `CargadorProductosTxt`. |
| `ServicioProducto` ya no posee eventos de stock/vencimiento | Cumple | Los posee `ServicioMonitoreoProductos`. |
| El monitor no depende de `ServicioProducto` | Cumple | Recibe `IEnumerable<Producto>` por operación. |
| Los cargadores TXT implementan sus interfaces | Cumple | Realizaciones UML en las vistas integrada y 03. |
| `CargadorProductosTxt` no instancia `MedicamentoCapsula` | Cumple | Delega en `ICreadorProducto`. |
| El creador de cápsulas implementa `ICreadorProducto` | Cumple | Realización UML y firma `Crear`. |
| `Program` aparece como composition root | Cumple | Estereotipo, dependencias de creación y nota explícita. |
| `ServicioVenta` está separado de consola | Cumple | No depende de `Console`; `Program` conserva interacción. |
| No hay cargadores JSON/API reales | Cumple | Solo existen los tres adaptadores TXT aprobados. |
| No hay creadores futuros implementados | Cumple | Solo aparece `CreadorMedicamentoCapsula`. |
| Se conserva la jerarquía real | Cumple | Jerarquías grises en UML-04, sin subtipos nuevos. |
| Multiplicidades coherentes | Cumple | `1` para dependencias retenidas y `0..*` para listas en memoria. |
| Identificadores coinciden con C# | Cumple | Inventario AS-IS contrastado; elementos nuevos coinciden con los ADR. |
| Diagramas legibles y sin líneas sobre textos | Cumple | Inspección de los cinco PNG a resolución completa. |
| Ninguna clase queda cortada | Cumple | Inspección visual y dimensiones registradas. |
| Leyenda de colores visible | Cumple | Incluida en cada uno de los cinco diagramas. |

No se compiló ni ejecutó la aplicación porque esta entrega modifica únicamente
documentación TO-BE y no cambia el código fuente de producción.
