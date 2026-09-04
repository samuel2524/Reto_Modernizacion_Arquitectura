# Ficha de patrón: Template Method

## Patrón y punto de dolor que resuelve

P-04: `CargadorProductosTxt.cs:22-63`, `CargadorClientesTxt.cs:13-43` y `CargadorUsuariosTxt.cs:13-45` repiten existencia del archivo, lectura, separación, conversión, agregado y manejo de errores. Una modificación común exige intervenir tres archivos.

## Alternativas evaluadas

**No hacer nada:** mantiene las tres copias. **Función estática genérica con parser y mensaje:** viable; puede centralizar todo el algoritmo. Se prefiere expresar los pasos especializados como miembros obligatorios de los cargadores existentes; no se atribuye una incapacidad a la alternativa. **Composición con parser:** también viable; permite sustitución independiente, a cambio de nuevos colaboradores y su conexión. **Template Method:** adoptado porque el orden es común y cada cargador ya representa una especialización estable.

## Qué sale y qué entra

Sale la copia del algoritmo en los tres cargadores. Entra `CargadorTxt<T>` con `Cargar(string ruta, ICollection<T> destino)`. La plantilla controla el orden y delega `ParsearCampos(string[] campos) : T`; cada subclase proporciona su mensaje. No se elimina ninguna clase ni interfaz pública.

## Cómo se relaciona

`Program` sigue construyendo los cargadores concretos. Productos, clientes y usuarios usan sus interfaces `ICargador*`; los servicios no conocen la base. `Cargar` comprueba el archivo, lee, recorre, hace `Split`, convierte, agrega y devuelve el mensaje; mantiene el manejo de errores actual. Productos conserva el selector de creadores. Clientes construye el convenio opcional de SC-3 y admite las filas antiguas de cuatro campos. No tiene relación directa con Composite.

## Impacto

Se crea una clase base y se modifican tres cargadores. `CargadorClientesTxt` participa también en SC-3, pero se cuenta una sola vez en la tabla. Se conservan los mensajes de carga, el comportamiento de SC-1 y la venta normal. No se implementa SC-2. Un ajuste común posterior se realiza en la base.

## Qué cuesta

La herencia obliga a leer base y subclase y hace que un cambio común afecte a los tres cargadores. Todos los pasos especializados deben ser cumplibles; no se introducen implementaciones vacías. La función genérica sigue siendo una alternativa técnicamente válida; la preferencia por herencia es una decisión explícita de organización.

## Origen

B-08 registra la aceptación del equipo. Esta revisión asistida corrige el descarte de la utilidad estática; la precisión debe reflejarse en la bitácora en preparación.
