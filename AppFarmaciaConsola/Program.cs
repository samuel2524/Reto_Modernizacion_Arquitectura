using BibFarmacia.Aspectos;
using BibFarmacia.Factories;
using BibFarmacia.Interfaces;
using BibFarmacia.Servicios;

Console.Title = "Sistema Farmacia";

CreadorMedicamentoCapsula creadorMedicamento =
    new CreadorMedicamentoCapsula();

CreadorCosmetico creadorCosmetico =
    new CreadorCosmetico();

CreadorComestible creadorComestible =
    new CreadorComestible();

IDictionary<string, ICreadorProducto> creadores =
    new Dictionary<string, ICreadorProducto>
    {
        ["medicamento"] = creadorMedicamento,
        ["cosmetico"] = creadorCosmetico,
        ["comestible"] = creadorComestible
    };

SelectorCreadorProducto selectorProductos =
    new SelectorCreadorProducto(
        creadores);

CargadorProductosTxt cargadorProductos =
    new CargadorProductosTxt(
        selectorProductos);

CargadorClientesTxt cargadorClientes =
    new CargadorClientesTxt();

CargadorUsuariosTxt cargadorUsuarios =
    new CargadorUsuariosTxt();

ServicioProducto servicioProducto =
    new ServicioProducto(
        cargadorProductos);

ServicioCliente servicioCliente =
    new ServicioCliente(
        cargadorClientes);

ServicioUsuario servicioUsuario =
    new ServicioUsuario(
        cargadorUsuarios);

ServicioMovimiento servicioMovimiento =
    new ServicioMovimiento();

ServicioVenta servicioVenta =
    new ServicioVenta(
        servicioProducto,
        servicioMovimiento);

ServicioFidelizacion servicioFidelizacion =
    new ServicioFidelizacion();

ServicioMonitoreoProductos servicioMonitoreoProductos =
    new ServicioMonitoreoProductos();

// ================= EVENTOS =================

servicioMonitoreoProductos.EventoStock.StockMinimo +=
    mensaje =>
    {
        Console.ForegroundColor =
            ConsoleColor.Red;

        Console.WriteLine(mensaje);

        Console.ResetColor();
    };

servicioMonitoreoProductos.EventoVencimiento.Vencimiento +=
    mensaje =>
    {
        Console.ForegroundColor =
            ConsoleColor.Yellow;

        Console.WriteLine(mensaje);

        Console.ResetColor();
    };

servicioFidelizacion.EventoPuntos.PuntosAcumulados +=
    mensaje =>
    {
        Console.ForegroundColor =
            ConsoleColor.Green;

        Console.WriteLine(mensaje);

        Console.ResetColor();
    };

servicioMovimiento.EventoMovimiento
    .MovimientoRegistrado +=
    mensaje =>
    {
        Console.ForegroundColor =
            ConsoleColor.Cyan;

        Console.WriteLine(mensaje);

        Console.ResetColor();
    };

// ================= CARGA TXT =================

Console.ForegroundColor =
    ConsoleColor.DarkGreen;

Console.WriteLine(
    "Cargando información del sistema...\n");

Console.ResetColor();

Console.WriteLine(
    servicioProducto.CargarDesdeArchivo(
        "productos.txt"));

Console.WriteLine(
    servicioCliente.Cargar(
        "clientes.txt"));

Console.WriteLine(
    servicioUsuario.Cargar(
        "usuarios.txt"));

Console.WriteLine();

// ================= LOGIN =================

Console.ForegroundColor =
    ConsoleColor.Blue;

Console.WriteLine(
    "=========== LOGIN ===========");

Console.ResetColor();

Console.Write("Usuario: ");
string user =
    Console.ReadLine()!;

Console.Write("Contraseña: ");
string password =
    Console.ReadLine()!;

bool login =
    servicioUsuario.Login(
        user,
        password);

if (!login)
{
    Console.ForegroundColor =
        ConsoleColor.Red;

    Console.WriteLine(
        "\nAcceso denegado");

    Console.ResetColor();

    return;
}

Console.ForegroundColor =
    ConsoleColor.Green;

Console.WriteLine(
    "\nLogin correcto");

Console.ResetColor();

// ================= ALERTAS =================

servicioMonitoreoProductos.VerificarStock(
    servicioProducto.ObtenerProductos());

servicioMonitoreoProductos.VerificarVencimiento(
    servicioProducto.ObtenerProductos());

// ================= MENÚ =================

int opcion = 0;

while (opcion != 7)
{
    Console.ForegroundColor =
        ConsoleColor.Magenta;

    Console.WriteLine("\n==============================");
    Console.WriteLine("      SISTEMA FARMACIA");
    Console.WriteLine("==============================");

    Console.ResetColor();

    Console.WriteLine("1. Ver productos");
    Console.WriteLine("2. Ver clientes");
    Console.WriteLine("3. Buscar producto");
    Console.WriteLine("4. Registrar venta");
    Console.WriteLine("5. Acumular puntos");
    Console.WriteLine("6. Ver alertas");
    Console.WriteLine("7. Salir");

    Console.Write("\nSeleccione opción: ");

    opcion =
        int.Parse(Console.ReadLine()!);

    switch (opcion)
    {
        case 1:

            Console.ForegroundColor =
                ConsoleColor.Cyan;

            Console.WriteLine(
                "\n===== PRODUCTOS =====");

            Console.ResetColor();

            Console.WriteLine(
                "Nombre\t\tStock\tPrecio");

            Console.WriteLine(
                "-----------------------------------");

            foreach (var producto in
                servicioProducto.ObtenerProductos())
            {
                Console.WriteLine(
                    $"{producto.Nombre}\t\t" +
                    $"{producto.Stock}\t" +
                    $"{producto.Precio}");
            }

            break;

        case 2:

            Console.ForegroundColor =
                ConsoleColor.Green;

            Console.WriteLine(
                "\n===== CLIENTES =====");

            Console.ResetColor();

            foreach (var cliente in
                servicioCliente.ObtenerClientes())
            {
                Console.WriteLine(
                    $"{cliente.Nombre} - " +
                    $"Puntos: {cliente.Puntos}");
            }

            break;

        case 3:

            Console.Write(
                "\nIngrese nombre producto: ");

            string nombre =
                Console.ReadLine()!;

            var productoBuscado =
                servicioProducto
                .ObtenerProductos()
                .FirstOrDefault(p =>
                    p.Nombre.ToLower()
                    .Contains(nombre.ToLower()));

            if (productoBuscado != null)
            {
                Console.WriteLine(
                    $"\nProducto: " +
                    $"{productoBuscado.Nombre}");

                Console.WriteLine(
                    $"Precio: " +
                    $"{productoBuscado.Precio}");

                Console.WriteLine(
                    $"Stock: " +
                    $"{productoBuscado.Stock}");
            }
            else
            {
                Console.WriteLine(
                    "\nProducto no encontrado");
            }

            break;

        case 4:

            Console.Write(
                "\nNombre producto: "); //Responsabilidad Interfaz de usuario

            string nombreVenta =
                Console.ReadLine()!;

            var productoVenta =
                servicioVenta
                .BuscarProducto(
                    nombreVenta);

            if (productoVenta != null)
            {
                Console.Write(
                    "Cantidad: ");

                int cantidad =
                    int.Parse(
                        Console.ReadLine()!);

                string resultadoVenta =
                    servicioVenta.Vender(
                        productoVenta,
                        cantidad);

                Console.WriteLine(
                    $"\n{resultadoVenta}");
            }
            else
            {
                Console.WriteLine(
                    "\nProducto no encontrado");
            }

            break;

        case 5:

            Console.Write(
                "\nNombre cliente: ");

            string nombreCliente =
                Console.ReadLine()!;

            var clientePuntos =
                servicioCliente
                .ObtenerClientes()
                .FirstOrDefault(c =>
                    c.Nombre.ToLower()
                    .Contains(
                        nombreCliente.ToLower()));

            if (clientePuntos != null)
            {
                Console.Write(
                    "Puntos: ");

                int puntos =
                    int.Parse(
                        Console.ReadLine()!);

                servicioFidelizacion
                    .AcumularPuntos(
                        clientePuntos,
                        puntos);
            }
            else
            {
                Console.WriteLine(
                    "\nCliente no encontrado");
            }

            break;

        case 6:

            Console.WriteLine(
                "\nVerificando alertas...");

            servicioMonitoreoProductos
                .VerificarStock(
                    servicioProducto
                        .ObtenerProductos());

            servicioMonitoreoProductos
                .VerificarVencimiento(
                    servicioProducto
                        .ObtenerProductos());

            break;

        case 7:

            Console.ForegroundColor =
                ConsoleColor.Red;

            Console.WriteLine(
                "\nSaliendo del sistema...");

            Console.ResetColor();

            break;

        default:

            Console.WriteLine(
                "\nOpción inválida");

            break;
    }
}

Console.WriteLine(
    "\nFIN DEL SISTEMA");
