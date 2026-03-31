namespace Biobrain.Application.Reports.UsageReport;

/*
https://trello.com/c/JuooCPQ6/523-1-colours-for-usage-report-chemistry

We can have the same colours for: VCE / HSC / SACE / AP courses, as they never occur together   
Different colours for IB, Senior, Year 10 Science

Therefore colours should be as follows:

Biology colours:
1. VCE / HSC / SACE / AP - #006193 (changes required)
2. IB - #00A0DD - (no change)
3. Senior - #2dc4ff - (no change)

Chemistry colours:
1. VCE / SACE - #125621 (no change)
2. IB - #22A23E (no change)
3. Senior - #3dd55d (no change)

Physics colours:
1. VCE / SACE - #287A86 (no change)
2. IB - #38afc1 (no change)
3. Senior - need a different colour here
 */

public class UsageReportSubjects
{
    public ReportKeyData BiologyVce = new() {Name = "VCE Biology", Color = "#006193" };
    public ReportKeyData BiologyIb = new() {Name = "IB Biology", Color = "#00A0DD" };
    public ReportKeyData BiologySenior = new() {Name = "Senior Biology", Color = "#2dc4ff" };
    public ReportKeyData BiologySace = new() {Name = "SACE Biology", Color = "#006193" };
    public ReportKeyData BiologyAp = new() { Name = "AP Biology", Color = "#006193" };
    public ReportKeyData BiologyHsc = new() { Name = "HSC Biology", Color = "#006193" };

    public ReportKeyData ChemistryVce = new() {Name = "VCE Chemistry", Color = "#125621" };
    public ReportKeyData ChemistryIb = new() {Name = "IB Chemistry", Color = "#22A23E" };
    public ReportKeyData ChemistrySenior = new() {Name = "Senior Chemistry", Color = "#3dd55d" };
    public ReportKeyData ChemistrySace = new() {Name = "SACE Chemistry", Color = "#125621" };
    public ReportKeyData ChemistryAp = new() {Name = "AP Chemistry", Color = "#23e44e" };

    public ReportKeyData PhysicsVce = new() {Name = "VCE Physics", Color = "#287A86" };
    public ReportKeyData PhysicsIb = new() {Name = "IB Physics", Color = "#38afc1" };
    public ReportKeyData PhysicsSenior = new() {Name = "Senior Physics", Color = "#287A86" };
    public ReportKeyData PhysicsSace = new() {Name = "SACE Physics", Color = "#287A86" };
    public ReportKeyData PhysicsAp = new() {Name = "AP Physics", Color = "#287A86" };

    public ReportKeyData Year10 = new() {Name = "Yr 10 Science", Color = "#ffbf00" };

    public ReportKeyData Life = new() { Name = "Life Science", Color = "#ffbf00" };
}

public class ReportKeyData
{
    public string Name { get; set; }
    public string Color { get; set; }
}