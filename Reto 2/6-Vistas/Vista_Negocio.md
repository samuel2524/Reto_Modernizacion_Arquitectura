# Vista para el negocio

Para: la Dirección de Ingeniería y quien aprueba el presupuesto.

## Qué le vamos a hacer al sistema y qué no cambia

No tocamos nada de lo que el sistema ya hace hoy: cada venta, cada cálculo y
cada alerta de inventario siguen funcionando exactamente igual que antes.
Lo comprobamos ejecutando los mismos doce escenarios de prueba que se usaron
en la entrega anterior y comparando la salida línea por línea: coincide al
cien por ciento.

Lo único nuevo es la capacidad de manejar los convenios comerciales que
ustedes pidieron: descuentos y crédito con empresas, bancos, cooperativas,
universidades y colegios. Es una opción adicional, no reemplaza la venta
normal.

## Dónde se está yendo hoy el tiempo y el dinero

Cada vez que aparece un tipo de alerta de inventario nuevo, hoy hay que
tocar varios archivos a la vez para agregarla, y nadie puede simplemente
sumar una regla nueva sin volver a armar esa parte completa.

La forma en que hoy se cargan los archivos de productos, clientes y
usuarios repite el mismo procedimiento tres veces por separado. Si mañana
cambia el formato de esos archivos, hay que corregirlo tres veces en vez de
una, con el riesgo de que alguna copia quede desactualizada.

La venta tampoco tenía ningún lugar preparado para aplicar una regla
comercial distinta según el cliente. Si ustedes hubieran pedido el manejo
de convenios sobre el diseño anterior, esa lógica habría tenido que meterse
a la fuerza dentro del proceso central de venta, haciéndolo más frágil con
cada convenio nuevo.

## Qué gana el negocio

Agregar una alerta de inventario nueva deja de obligar a tocar varios
archivos: se suma una pieza nueva sin rehacer las que ya funcionan. Eso baja
el tiempo de respuesta la próxima vez que pidan una alerta adicional.

Agregar un convenio nuevo con otra entidad no obliga a tocar el proceso de
venta: se agrega por separado y se conecta en un solo punto. El riesgo de
que un convenio nuevo rompa una venta normal queda controlado.

En general, el riesgo de que un cambio futuro rompa algo que ya funciona
baja, porque cada parte del sistema queda más aislada de las demás.

## Qué cuesta

Este cambio no agrega tiempo de desarrollo visible del lado del cliente ni
de la operación diaria: es trabajo interno de organización, no una
funcionalidad aparte además del manejo de convenios.

Sí cuesta algo de complejidad interna: hay más piezas pequeñas que antes
(donde había una sola pieza encargada de las alertas de inventario, ahora
hay varias piezas más chicas que se combinan). Eso se paga una sola vez
ahora, para no pagarlo cada vez que aparezca una alerta o un convenio
nuevo.

## Qué riesgos hay

El riesgo principal es que, al reorganizar cómo se arman las alertas de
inventario, cambie por accidente el orden o el contenido de un mensaje que
el negocio ya conoce. Lo mitigamos comparando la salida del sistema, línea
por línea, contra la versión anterior, antes de entregar.

Otro riesgo es que el sistema no arranque si algo queda mal conectado
internamente. Lo mitigamos probando después de cada cambio, no solo al
final.

Ambos riesgos se muestran resueltos en la sustentación, con el sistema
corriendo en vivo.

## Qué necesitamos del negocio

Confirmar los datos reales de los convenios que se van a manejar (con qué
entidades, qué porcentaje de descuento, qué cupo de crédito), porque hoy se
trabaja con datos de ejemplo mientras se valida el diseño.

Aprobar que el alcance de esta entrega se quede en la lógica interna del
sistema: no se está tocando base de datos, ni la interfaz, ni la
conectividad con otros sistemas.

## Qué pasa si no se hace

El sistema sigue funcionando para lo que ya hace, pero cada solicitud nueva
(una alerta, un convenio, un producto de otro tipo) va a seguir costando
más tiempo del necesario, porque cada una obliga a tocar partes que ya
funcionan en vez de sumar una pieza aparte. El costo no desaparece: se
sigue acumulando en cada entrega futura.
