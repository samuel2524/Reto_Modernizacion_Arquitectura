# Ejecuta los casos de caracterizacion contra el sistema ORIGINAL y el REDISEÑADO
# y compara sus salidas. La conducta observable se preserva si todas coinciden.
#
# Uso:  .\caracterizacion.ps1 -Repo C:\ruta\al\repo -Salida C:\ruta\de\salida
#
# Requisito de comparacion: el sistema rediseñado se ejecuta con el MISMO
# inventario de diez medicamentos que el original. El archivo del rediseñado
# lleva la columna de tipo que introdujo ADR-006; el original no la entiende.
# Sin esta equivalencia la comparacion no seria valida.

param(
    [Parameter(Mandatory = $true)][string]$Repo,
    [Parameter(Mandatory = $true)][string]$Salida
)

$ErrorActionPreference = "Stop"
$env:DOTNET_ROLL_FORWARD = "Major"

$orig = Join-Path $Repo "Codigo_No_Modificado\AppFarmaciaConsola"
$nuevo = Join-Path $Repo "Trabajo Farmacia\04-src\AppFarmaciaConsola"

# --- casos: nombre, descripcion, y las teclas que se envian por consola -----
$casos = @(
    @{ id = "CC-01"; desc = "Arranque: carga de los tres archivos y alertas iniciales"
       in = @("admin", "1234", "7") },
    @{ id = "CC-02"; desc = "Login con credenciales validas"
       in = @("admin", "1234", "7") },
    @{ id = "CC-03"; desc = "Login con credenciales invalidas"
       in = @("admin", "9999") },
    @{ id = "CC-04"; desc = "Listar productos: orden y formato del inventario"
       in = @("admin", "1234", "1", "7") },
    @{ id = "CC-05"; desc = "Listar clientes con sus puntos"
       in = @("admin", "1234", "2", "7") },
    @{ id = "CC-06"; desc = "Buscar un producto existente"
       in = @("admin", "1234", "3", "Dolex", "7") },
    @{ id = "CC-07"; desc = "Buscar un producto inexistente"
       in = @("admin", "1234", "3", "NoExiste", "7") },
    @{ id = "CC-08"; desc = "Venta con stock suficiente: descuento y movimiento"
       in = @("admin", "1234", "4", "Omeprazol", "5", "1", "7") },
    @{ id = "CC-09"; desc = "Venta MAYOR al stock: el inventario queda negativo (H-07)"
       in = @("admin", "1234", "4", "Amoxicilina", "54", "1", "7") },
    @{ id = "CC-10"; desc = "Acumular puntos a un cliente y ver el evento"
       in = @("admin", "1234", "5", "Carlos", "120", "2", "7") },
    @{ id = "CC-11"; desc = "Ver alertas de stock minimo y vencimiento"
       in = @("admin", "1234", "6", "7") },
    @{ id = "CC-12"; desc = "Opcion de menu inexistente"
       in = @("admin", "1234", "99", "7") }
)

New-Item -ItemType Directory -Force -Path $Salida | Out-Null
$dirOrig = Join-Path $Salida "salidas-original"
$dirNuevo = Join-Path $Salida "salidas-redisenado"
New-Item -ItemType Directory -Force -Path $dirOrig, $dirNuevo | Out-Null

# --- inventario equivalente: los mismos diez medicamentos en ambos ----------
$prodOrig = Join-Path $orig "productos.txt"
$prodNuevo = Join-Path $nuevo "productos.txt"
$respaldo = Join-Path $Salida "productos-completo-con-SC1.txt"
Copy-Item $prodNuevo $respaldo -Force
$diez = Get-Content $prodOrig | ForEach-Object { "medicamento;$_" }
Set-Content $prodNuevo -Value $diez -Encoding utf8

function Ejecutar($carpeta, $entrada, $destino) {
    $tmp = Join-Path $env:TEMP "cc_in.txt"
    Set-Content $tmp -Value (($entrada -join "`r`n") + "`r`n") -Encoding ascii -NoNewline
    Push-Location $carpeta
    $texto = cmd /c "dotnet run --no-build < `"$tmp`" 2>&1"
    Pop-Location
    # se normaliza la fecha: el movimiento imprime DateTime.Now y cambia en cada corrida
    $texto = $texto -replace '\d{1,2}/\d{1,2}/\d{4}[^\r\n]*', '<FECHA-HORA>'
    Set-Content $destino -Value $texto -Encoding utf8
    return $texto
}

$resultados = @()
foreach ($c in $casos) {
    $a = Ejecutar $orig  $c.in (Join-Path $dirOrig  "$($c.id).txt")
    $b = Ejecutar $nuevo $c.in (Join-Path $dirNuevo "$($c.id).txt")
    $igual = ((($a -join "`n").Trim()) -eq (($b -join "`n").Trim()))
    $resultados += [PSCustomObject]@{
        Caso = $c.id; Escenario = $c.desc
        Resultado = $(if ($igual) { "IDENTICA" } else { "DIFIERE" })
    }
    "{0}  {1,-58} {2}" -f $c.id, $c.desc, $resultados[-1].Resultado
}

# --- se restaura el inventario completo con los productos de SC-1 -----------
Copy-Item $respaldo $prodNuevo -Force
Remove-Item $respaldo -Force

$ok = ($resultados | Where-Object { $_.Resultado -eq "IDENTICA" }).Count
""
"RESULTADO: $ok de $($resultados.Count) casos con salida identica"
$resultados | Export-Csv (Join-Path $Salida "resumen-caracterizacion.csv") -NoTypeInformation -Encoding utf8
if ($ok -ne $resultados.Count) { exit 1 }
