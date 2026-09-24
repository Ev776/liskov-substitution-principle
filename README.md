# Caso Práctico: Principio de Sustitución de Liskov (LSP)

Este proyecto muestra cómo aplicar el **Principio de Sustitución de Liskov** en un escenario común del desarrollo de software: el procesamiento de pagos en una tienda digital.

---

## 1. El Problema: Cuando la herencia engaña al sistema

El Principio de Sustitución de Liskov dice que si una clase deriva de otra, el programa debe poder usar la clase hija en lugar de la padre sin que nada falle ni cambie de forma inesperada.

El error más común es forzar una relación de herencia solo porque los nombres se parecen, aunque el comportamiento no sea el mismo:

```csharp
// Clase base general
public abstract class MetodoPago
{
    public abstract void Cobrar(decimal monto);
}

// Funciona bien: la tarjeta cobra al momento
public class TarjetaCredito : MetodoPago
{
    public override void Cobrar(decimal monto)
    {
        Console.WriteLine($"Cobro exitoso de ${monto} con Tarjeta.");
    }
}

// Rompe Liskov: una transferencia no puede cobrar inmediatamente
public class TransferenciaBancaria : MetodoPago
{
    public override void Cobrar(decimal monto)
    {
        // Provoca un error en ejecución porque no puede cumplir la promesa del padre
        throw new NotSupportedException("Error: Una transferencia no cobra de inmediato.");
    }
}
