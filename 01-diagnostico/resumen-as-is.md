# Resumen del sistema AS-IS

## 1. Descripción general

Solución Farmacia es un prototipo de gestión farmacéutica desarrollado en C#
sobre .NET 8. La solución está formada por `BibFarmacia`, que contiene el
modelo y los servicios, y `AppFarmaciaConsola`, que contiene el punto de entrada
y la interacción con el usuario. No utiliza paquetes externos ni una base de
datos; la carga inicial se realiza desde archivos TXT y el estado se conserva
en listas en memoria.

El sistema permite cargar productos, clientes y usuarios, autenticar usuarios,
consultar información, registrar ventas simples, descontar stock, registrar
movimientos, acumular puntos y mostrar alertas de stock mínimo y vencimiento.
Sin embargo, los cambios de stock, puntos y movimientos se pierden al cerrar la
aplicación.

La solución contiene 22 clases declaradas, de las cuales 2 son abstractas, 2
interfaces, 2 enumeraciones, 4 delegates anidados y 4 eventos. Su organización
actual incluye entidades, servicios, eventos, interfaces, enumeraciones, una
factoría y clases auxiliares de autenticación y validación. Algunos componentes,
como `ProductoFactory`, `ServicioDescuento`, `ServicioNotificacion`,
`AspectoValidacion` y `MedicamentoLiquido`, existen en el código, pero no están
integrados en el flujo principal.

## 2. Diagramas UML del estado actual

El diagrama UML AS-IS se dividió en cinco vistas para evitar concentrar todas
las clases y relaciones en una sola imagen difícil de interpretar:

1. **Vista general:** presenta los proyectos, namespaces, clases, interfaces y
   relaciones arquitectónicas principales. Sirve como índice de las otras
   cuatro vistas.
2. **Dominio y entidades:** detalla las jerarquías de `Persona` y `Producto`,
   junto con clientes, usuarios, medicamentos, laboratorios, movimientos y las
   enumeraciones utilizadas por los distintos tipos de medicamento.
3. **Servicios e interfaces:** muestra los servicios que administran usuarios,
   productos, clientes y movimientos, además de los contratos de descuento y
   notificación y sus relaciones con entidades y eventos.
4. **Eventos y delegados:** explica cómo los servicios publican alertas de
   stock, vencimiento, puntos y movimientos, y cómo `Program` se suscribe a
   ellas para mostrar los mensajes.
5. **Construcción y soporte:** representa el punto de entrada, la creación
   directa de servicios y movimientos, `ProductoFactory`, y las clases
   auxiliares de autenticación y validación.

Los diagramas describen únicamente lo que existe actualmente. No incorporan
capas, repositorios, interfaces ni patrones que todavía no estén implementados.

## 3. Inventario de hallazgos y puntos de dolor

El documento `Inventario_de_hallazgos_y_puntos_de_dolor_AS-IS.docx` reúne los
principales problemas detectados, su ubicación, el principio comprometido, el
impacto para el negocio, la severidad y el origen del hallazgo.

De forma general, el inventario señala cuatro situaciones:

- El flujo de venta está dentro de `Program.cs` y mezcla interacción por
  consola con búsqueda, descuento de inventario y registro del movimiento.
- La carga de productos siempre construye cápsulas con relleno de gel, aunque
  el dominio también contempla medicamentos líquidos.
- `ServicioCliente` concentra el almacenamiento de clientes, la carga desde
  archivos y la acumulación de puntos.
- Los servicios de productos, clientes y usuarios dependen directamente de
  `System.IO.File`, archivos TXT, separadores y columnas posicionales.

Los puntos de dolor priorizados son la mezcla de reglas de venta con la consola,
el acoplamiento de los servicios al formato TXT y la carga limitada a cápsulas.
Sus consecuencias principales son el riesgo de inconsistencias entre ventas,
stock y movimientos, la dificultad de reemplazar los archivos por otra forma
de persistencia y la imposibilidad de cargar todas las presentaciones que el
propio modelo admite. El documento incluye además un apartado destinado al
hallazgo de la IA que deberá ser refutado, pero este todavía está pendiente de
identificación y documentación.

## 4. Mapa de dependencias AS-IS

El mapa de dependencias organiza el sistema en tres bloques. El primero muestra
el flujo principal: `Program` crea servicios concretos y estos almacenan las
entidades. También evidencia dependencias directas hacia consola, reloj,
`System.IO.File` y los formatos TXT.

El segundo bloque representa el modelo y sus políticas: las jerarquías de
personas y productos, las relaciones con laboratorio y movimiento, las
enumeraciones, la validación y la factoría. En este bloque se observa que
`Producto` conoce la consola y que `ProductoFactory` y las reglas de vencimiento
dependen directamente del reloj del sistema.

El tercer bloque muestra el desacoplamiento que ya existe mediante eventos e
interfaces. Los servicios disparan eventos y `Program` conecta los manejadores,
pero `IDescuento` e `IServicioNotificacion`, aunque tienen implementaciones, no
son utilizados por los casos de uso actuales.

En conjunto, el mapa permite diferenciar políticas de alto nivel, módulos que
mezclan negocio e infraestructura, detalles técnicos, presentación y mecanismos
de desacoplamiento. Su conclusión principal es que la dirección de dependencias
todavía no está claramente separada: varias reglas y servicios dependen de
detalles concretos como archivos, consola y reloj.

## 5. Trabajo pendiente

Los siguientes puntos se registran únicamente como tareas pendientes; no forman
parte del trabajo realizado en este resumen:

- Mejorar considerablemente los diagramas UML del estado actual del sistema.
- Mejorar considerablemente el UML del mapa de dependencias.
- Encontrar, verificar y documentar el hallazgo de la IA que será refutado.

## 6. Alcance

Este documento resume el diagnóstico y los artefactos existentes. No propone un
diseño futuro, no resuelve las tareas pendientes y no implica modificaciones al
código fuente del sistema.
