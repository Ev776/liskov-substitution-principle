Caso Práctico: Principio de Sustitución de Liskov (LSP)

Este proyecto muestra cómo aplicar el Principio de Sustitución de Liskov (LSP) en un escenario real: un sistema de procesamiento de compras y pagos.

1. El Problema: Romper el contrato de la clase padre

El Principio de Sustitución de Liskov establece que los objetos de un programa deben poder ser reemplazados por instancias de sus subtipos sin alterar el correcto funcionamiento del sistema.

Un error común al abusar de la herencia es forzar a una subclase a cumplir con un comportamiento que en realidad no posee:

// Clase base general
public abstract class MetodoPago
{
    public abstract void Cobrar(decimal monto);
}

// Subclase que sí puede cobrar al momento
public class TarjetaCredito : MetodoPago
{
    public override void Cobrar(decimal monto)
    {
        Console.WriteLine($"Cobro exitoso de ${monto} con Tarjeta.");
    }
}

// ❌ Rompe LSP: una transferencia bancaria no se procesa de inmediato
public class TransferenciaBancaria : MetodoPago
{
    public override void Cobrar(decimal monto)
    {
        // Provoca un error en ejecución porque no puede cumplir la promesa del padre
        throw new NotSupportedException("Una transferencia no puede cobrar de inmediato.");
    }
}


¿Por qué esto es un problema en producción?

Cualquier servicio del sistema que espere un MetodoPago asume que al invocar Cobrar() la transacción se completará. Si el usuario selecciona transferencia bancaria, el programa lanzará una excepción imprevista en tiempo de ejecución, obligando a los desarrolladores a parchar el código con validaciones del estilo:

if (metodo is TransferenciaBancaria) { ... }


2. La Solución: Separación de contratos por comportamiento

En lugar de crear una jerarquía rígida e irreal, definimos contratos claros y específicos mediante interfaces:

IPagoInmediato: Para métodos que pueden procesar una transacción de forma síncrona (Tarjeta de Crédito, PayPal, Apple Pay).

IPagoDiferido: Para métodos que requieren una referencia previa o trámite externo (Transferencia, OXXO, SPEI).

public interface IPagoInmediato
{
    void Cobrar(decimal monto);
}

public interface IPagoDiferido
{
    string GenerarReferencia(decimal monto);
}


Al diseñar de esta forma, el procesador de órdenes solo interactúa con contratos que garantizan la ejecución inmediata:

public class ProcesadorCompras
{
    // Cumple LSP al 100%: cualquier subtipo de IPagoInmediato 
    // puede usarse aquí sin sorpresas ni errores de ejecución.
    public void ProcesarOrden(IPagoInmediato metodo, decimal total)
    {
        metodo.Cobrar(total);
    }
}


3. Beneficios Clave Obtenidos

Cero fallos sorpresa en producción: Se eliminan llamadas a funciones que lanzan errores de "acción no implementada".

Código limpio y sin parches: No se requieren comprobaciones manuales de tipo (is / instanceof / switch) para saber con qué objeto se está operando.

Pruebas unitarias confiables: Facilita reemplazar implementaciones reales por mocks o fakes de prueba sin alterar el flujo del sistema.

Escalabilidad real: Agregar nuevos métodos de pago solo implica implementar la interfaz adecuada, sin tocar ni arriesgar el código central que ya funciona.
