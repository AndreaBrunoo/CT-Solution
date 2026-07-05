# Lambda Guide – Metodi LINQ più utilizzati

Questa guida raccoglie i metodi LINQ più usati nei Service XPO, con esempi chiari e sintetici.

---

SELECT

Estrae una proprietà o trasforma un oggetto.

var emails = users.Select(u => u.Email).ToList();

var dtoList = xpoList.Select(x => Mapper.ToDto(Mapper.ToDomain(x))).ToList();

---

WHERE

Filtra gli elementi.

var activeUsers = users.Where(u => u.IsActive).ToList();

var logs = uow.Query<XpoWorkLog>()
              .Where(w => w.IdEmployee == employeeId)
              .ToList();

---

FIRST / FIRSTORDEFAULT

Prende il primo elemento (o null se non esiste).

var user = users.FirstOrDefault(u => u.Email == email);

var log = uow.Query<XpoWorkLog>()
             .FirstOrDefault(w => w.Id == id);

---

SINGLE / SINGLEORDEFAULT

Usato quando ci deve essere esattamente un elemento.

var config = configs.SingleOrDefault(c => c.Key == "Theme");

---

ANY

Controlla se esiste almeno un elemento.

bool exists = users.Any(u => u.Email == email);

---

COUNT

Conta gli elementi.

int total = logs.Count();

int active = users.Count(u => u.IsActive);

---

ORDERBY / ORDERBYDESCENDING

Ordina gli elementi.

var ordered = users.OrderBy(u => u.Email).ToList();

var recent = logs.OrderByDescending(l => l.Date).ToList();

---

TAKE / SKIP

Paginazione.

var page = logs.OrderByDescending(l => l.Date)
               .Skip(10)
               .Take(10)
               .ToList();

---

GROUPBY

Raggruppa per una proprietà.

var grouped = logs.GroupBy(l => l.IdProject)
                  .Select(g => new 
                  {
                      ProjectId = g.Key,
                      TotalHours = g.Sum(x => x.HoursCounter)
                  })
                  .ToList();

---

DISTINCT

Rimuove duplicati.

var roles = users.Select(u => u.Role).Distinct().ToList();

---

SELECT MANY

Appiattisce collezioni annidate.

var allTasks = projects.SelectMany(p => p.Tasks).ToList();

---

TO LIST / TO ARRAY

Materializza la query.

var list = query.ToList();
var array = query.ToArray();

---

TO DICTIONARY

Crea un dizionario.

var dict = users.ToDictionary(u => u.Id, u => u.Email);

---

AGGREGATE

Riduce una collezione a un valore.

int totalHours = logs.Aggregate(0, (acc, l) => acc + l.HoursCounter);

---

MAX / MIN / SUM / AVERAGE

Operazioni matematiche.

var maxHours = logs.Max(l => l.HoursCounter);
var minHours = logs.Min(l => l.HoursCounter);
var sumHours = logs.Sum(l => l.HoursCounter);
var avgHours = logs.Average(l => l.HoursCounter);

---

JOIN

Unisce due collezioni.

var result = users.Join(
    roles,
    u => u.RoleId,
    r => r.Id,
    (u, r) => new { u.Email, r.Name }
).ToList();

---

Conclusione

Questa guida contiene tutte le operazioni LINQ più usate nei Service XPO.  
Puoi copiarla direttamente nel tuo file `.md` come riferimento rapido.