using System;

namespace SolidLspExample
{
    // =======================================================
    // 1. CONTRATOS (INTERFACES CON REGLAS CLARAS)
    // =======================================================

    /// <summary>
    /// Contrato para pagos que pueden cobrarse de forma inmediata.
    /// </summary>
    public interface IPagoInmediato
    {
        void Cobrar(decimal monto);
    }

    /// <summary>
    /// Contrato para pagos que requieren generar una referencia previa.
    /// </summary>
    public interface IPagoDiferido
    {
        string GenerarReferencia(decimal monto);
    }

    // =======================================================
    // 2. IMPLEMENTACIONES QUE RESPETAN LISKOV
    // =======================================================

    public class TarjetaCredito : IPagoInmediato
    {
        public void Cobrar(decimal monto)
        {
            Console.WriteLine($"[Tarjeta de Crédito] Cobro procesado exitosamente por: ${monto:F2}");
        }
    }

    public class PayPal : IPagoInmediato
    {
        public void Cobrar(decimal monto)
        {
            Console.WriteLine($"[PayPal] Token validado y cobro procesado por: ${monto:F2}");
        }
    }

    public class ApplePay : IPagoInmediato
    {
        public void Cobrar(decimal monto)
        {
            Console.WriteLine($"[Apple Pay] Pago biometrico completado por: ${monto:F2}");
        }
    }

    public class TransferenciaBancaria : IPagoDiferido
    {
        public string GenerarReferencia(decimal monto)
        {
            string referencia = $"SPEI-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";
            return referencia;
        }
    }

    // =======================================================
    // 3. CÓDIGO CLIENTE (CONSUMIDOR DE LSP)
    // =======================================================

    public class ProcesadorCompras
    {
        // Cumple el Principio de Sustitución de Liskov:
        // No importa si recibe Tarjeta, PayPal o ApplePay; todos responden al contrato
        // sin romper la ejecución, sin excepciones y sin condicionales de tipo.
        public void FinalizarCompra(IPagoInmediato metodoPago, decimal total)
        {
            Console.WriteLine("Iniciando procesamiento de la compra...");
            metodoPago.Cobrar(total);
            Console.WriteLine("Compra finalizada con exito.\n");
        }
    }

    // =======================================================
    // 4. EJECUCIÓN DEL PROGRAMA
    // =======================================================

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==================================================");
            Console.WriteLine(" DEMO: PRINCIPIO DE SUSTITUCION DE LISKOV (LSP)   ");
            Console.WriteLine("==================================================\n");

            var procesador = new ProcesadorCompras();

            // Sustitución transparente: diferentes subtipos sustituyen a la abstracción
            IPagoInmediato pago1 = new TarjetaCredito();
            IPagoInmediato pago2 = new PayPal();
            IPagoInmediato pago3 = new ApplePay();

            procesador.FinalizarCompra(pago1, 149.99m);
            procesador.FinalizarCompra(pago2, 85.50m);
            procesador.FinalizarCompra(pago3, 310.00m);

            // El flujo diferido queda protegido por su propia interfaz
            var transferencia = new TransferenciaBancaria();
            string refPago = transferencia.GenerarReferencia(1200.00m);
            Console.WriteLine($"[Transferencia Bancaria] Referencia generada: {refPago}");
            Console.WriteLine("Esperando recepcion del comprobante bancario...\n");

            Console.WriteLine("Demostracion concluida con exito.");
        }
    }
}
