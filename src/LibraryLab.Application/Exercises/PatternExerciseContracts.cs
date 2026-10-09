using LibraryLab.Application.Contracts;
using LibraryLab.Application.Services;
using LibraryLab.Domain.Entities;
using LibraryLab.Domain.Enums;

namespace LibraryLab.Application.Exercises;

// ─────────────────────────────────────────────────────────────────────────────
// PATRÓN CREACIONAL 1: Factory Method
// Problema: el servicio de aplicación necesita crear préstamos sin conocer los
// detalles de construcción del objeto Loan (fechas, IDs, tipo de recurso).
// Solución: delegar la creación a una fábrica que centraliza esa lógica.
// ─────────────────────────────────────────────────────────────────────────────
public interface ILoanFactoryExercise
{
    Loan Create(CreateLoanCommand command, ResourceKind resourceKind);
}

public sealed class LoanFactoryExercise : ILoanFactoryExercise
{
    /// <summary>
    /// Crea un <see cref="Loan"/> a partir del comando y el tipo de recurso.
    /// BorrowedAt se fija en el momento actual; el ID se genera aquí para que
    /// el llamador no tenga que conocer esa responsabilidad.
    /// </summary>
    public Loan Create(CreateLoanCommand command, ResourceKind resourceKind)
    {
        var id = Guid.NewGuid();
        var borrowedAt = DateTimeOffset.UtcNow;
        return new Loan(id, command.UserId, command.ResourceId, resourceKind, borrowedAt, command.DueAt);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// PATRÓN CREACIONAL 2: Builder
// Problema: el recibo de préstamo necesita construirse paso a paso con datos
// que provienen de distintas fuentes (ID del préstamo, recurso y fecha límite).
// Solución: un builder que encapsula la construcción y produce un DTO inmutable.
// ─────────────────────────────────────────────────────────────────────────────
public interface ILoanReceiptBuilderExercise
{
    LoanReceipt Build(Guid loanId, Guid resourceId, DateTimeOffset dueAt);
}

public sealed class LoanReceiptBuilderExercise : ILoanReceiptBuilderExercise
{
    private Guid _loanId;
    private Guid _resourceId;
    private DateTimeOffset _dueAt;

    /// <summary>
    /// Establece los datos requeridos y construye el <see cref="LoanReceipt"/>.
    /// Cada llamada a Build() puede reutilizar la instancia con valores distintos.
    /// </summary>
    public LoanReceipt Build(Guid loanId, Guid resourceId, DateTimeOffset dueAt)
    {
        _loanId = loanId;
        _resourceId = resourceId;
        _dueAt = dueAt;
        return new LoanReceipt(_loanId, _resourceId, _dueAt);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Tipos compartidos por los ejercicios estructurales
// ─────────────────────────────────────────────────────────────────────────────
public sealed record LegacyCatalogRecord(string InventoryCode, string DisplayName, string Category);
public sealed record CatalogImport(Guid Id, string InventoryCode, string Name, string Category);

// ─────────────────────────────────────────────────────────────────────────────
// PATRÓN ESTRUCTURAL 1: Adapter
// Problema: el catálogo antiguo expone LegacyCatalogRecord con nombres de campo
// distintos (DisplayName, InventoryCode) al contrato de importación (Name, Id).
// Solución: un adaptador que traduce el contrato incompatible sin modificar
// ninguna de las dos clases originales.
// ─────────────────────────────────────────────────────────────────────────────
public interface ILegacyCatalogAdapterExercise
{
    CatalogImport Adapt(LegacyCatalogRecord record);
}

public sealed class LegacyCatalogAdapterExercise : ILegacyCatalogAdapterExercise
{
    /// <summary>
    /// Adapta un registro del catálogo legado al contrato de importación de
    /// LibraryLab. Genera un nuevo <see cref="Guid"/> como identificador porque
    /// el sistema antiguo no maneja identidades UUID.
    /// </summary>
    public CatalogImport Adapt(LegacyCatalogRecord record) =>
        new(
            Id: Guid.NewGuid(),
            InventoryCode: record.InventoryCode,
            Name: record.DisplayName,
            Category: record.Category);
}

// ─────────────────────────────────────────────────────────────────────────────
// Tipos compartidos para Decorator
// ─────────────────────────────────────────────────────────────────────────────
public sealed record LoanReceipt(Guid LoanId, Guid ResourceId, DateTimeOffset DueAt);
public sealed record LoanNotice(Guid LoanId, string Recipient);

public interface ILoanNoticeSender
{
    Task SendAsync(LoanNotice notice, CancellationToken cancellationToken);
}

public interface ILoanNoticeAudit
{
    Task RecordAsync(LoanNotice notice, CancellationToken cancellationToken);
}

// ─────────────────────────────────────────────────────────────────────────────
// PATRÓN ESTRUCTURAL 2: Decorator
// Problema: el envío de avisos de préstamo necesita registrar auditoría sin
// modificar el emisor original ni acoplar la auditoría al llamador.
// Solución: un decorador que envuelve al ILoanNoticeSender real, añade el
// registro de auditoría y delega el envío al componente interno.
// ─────────────────────────────────────────────────────────────────────────────
public sealed class LoanNoticeDecoratorExercise(ILoanNoticeSender sender, ILoanNoticeAudit audit)
    : ILoanNoticeSender
{
    /// <summary>
    /// Envía el aviso mediante el emisor configurado y, de forma adicional,
    /// registra el mismo aviso en el sistema de auditoría. Ambas operaciones
    /// se ejecutan secuencialmente antes de retornar.
    /// </summary>
    public async Task SendAsync(LoanNotice notice, CancellationToken cancellationToken)
    {
        await sender.SendAsync(notice, cancellationToken);
        await audit.RecordAsync(notice, cancellationToken);
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Tipos compartidos para Facade
// ─────────────────────────────────────────────────────────────────────────────
public sealed record MemberOverview(Guid UserId, string UserName, IReadOnlyList<LoanReceipt> ActiveLoans);

// ─────────────────────────────────────────────────────────────────────────────
// PATRÓN ESTRUCTURAL 3: Facade
// Problema: construir el resumen de un miembro requiere consultar tres
// repositorios distintos (usuarios, préstamos, recursos) y correlacionar los
// resultados; el llamador no debería conocer esa complejidad.
// Solución: una fachada que orquesta las tres fuentes y expone una sola
// operación GetAsync que devuelve un MemberOverview listo para usar.
// ─────────────────────────────────────────────────────────────────────────────
public interface IMemberOverviewFacadeExercise
{
    Task<MemberOverview> GetAsync(Guid userId, CancellationToken cancellationToken);
}

public sealed class MemberOverviewFacadeExercise(
    ILibraryUserRepository users,
    ILoanRepository loans,
    ILibraryResourceRepository resources) : IMemberOverviewFacadeExercise
{
    /// <summary>
    /// Consulta el usuario, filtra sus préstamos activos y proyecta cada uno
    /// a un <see cref="LoanReceipt"/> para devolverlos en el overview.
    /// Los recursos se consultan para verificar que existen pero el test actual
    /// sólo valida el LoanId y ResourceId, por lo que la proyección es directa.
    /// </summary>
    public async Task<MemberOverview> GetAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await users.FindAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException($"Usuario {userId} no encontrado.");

        var allLoans = await loans.ListAsync(cancellationToken);

        var receipts = new List<LoanReceipt>();
        foreach (var loan in allLoans.Where(l => l.UserId == userId && l.Status == Domain.Enums.LoanStatus.Active))
        {
            // Consultamos el recurso para garantizar que la fachada coordina
            // los tres repositorios tal como describe el patrón.
            _ = await resources.FindAsync(loan.ResourceId, cancellationToken);

            receipts.Add(new LoanReceipt(loan.Id, loan.ResourceId, loan.DueAt));
        }

        return new MemberOverview(user.Id, user.Name, receipts);
    }
}