# Caso Práctico: Principio de Sustitución de Liskov (LSP)

Este proyecto muestra cómo aplicar el Principio de Sustitución de Liskov (LSP) en un escenario real: un sistema de procesamiento de compras y pagos.

---

## 1. El Problema: Romper el contrato de la clase padre

El Principio de Sustitución de Liskov establece que los objetos de un programa deben poder ser reemplazados por instancias de sus subtipos sin alterar el correcto funcionamiento del sistema.

Un error común al abusar de la herencia es forzar a una subclase a cumplir con un comportamiento que en realidad no posee:

```csharp
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

// Rompe LSP: una transferencia bancaria no se procesa de inmediato
public class TransferenciaBancaria : MetodoPago 
{ 
    public override void Cobrar(decimal monto) 
    { 
        // Provoca un error en ejecución porque no puede cumplir la promesa del padre
        throw new NotSupportedException("Una transferencia no puede cobrar de inmediato."); 
    } 
}
```

---

## 2. La Solución: Diseñar contratos adecuados (Interfaces)

En lugar de forzar a todas las clases a implementar una acción que no les corresponde, separamos las responsabilidades usando contratos específicos:

```csharp
// Interfaz para pagos procesados al instante
public interface IPagoInmediato
{
    void Cobrar(decimal monto);
}

// Interfaz para pagos que requieren trámite previo
public interface IPagoDiferido
{
    string GenerarReferencia(decimal monto);
}
```

Con este rediseño:
* `TarjetaCredito` y `PayPal` implementan `IPagoInmediato`.
* `TransferenciaBancaria` implementa `IPagoDiferido`.
* El carrito de compras (`CarritoCompra`) solo solicita un `IPagoInmediato`. Cualquier opción compatible puede sustituir a otra de forma transparente y segura.

---

## 3. Beneficios Obtenidos

1. **Cero sorpresas en ejecución:** Desaparecen las excepciones inesperadas por métodos no implementados.
2. **Código sin parches:** No se requieren verificaciones de tipo (`if / is / instanceof`) antes de invocar un método.
3. **Escalabilidad limpia:** Agregar nuevos métodos compatibles (como Apple Pay o saldo virtual) no exige tocar el código del carrito.
