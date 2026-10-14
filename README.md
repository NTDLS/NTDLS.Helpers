# NTDLS.Helpers

NTDLS.Helpers is a lightweight .NET library of small, focused utility helpers for everyday C# work: null-safety extension methods, best-effort string-to-type conversion, per-key thread synchronization, text and HTML cleanup, task error handling, and PBKDF2 password hashing. It targets .NET 8, 9, and 10 with no dependencies beyond `Microsoft.Extensions.Caching.Memory`.

📦 Be sure to check out the NuGet package: https://www.nuget.org/packages/NTDLS.Helpers

## When to use it

Reach for NTDLS.Helpers when you want small, well-tested building blocks instead of writing (or re-writing) the same defensive code in every project:

- **Null-safety without boilerplate** — replace `if (value == null) throw ...` checks with one-line extensions like `EnsureNotNull()` and `DefaultWhenNull()`, including nullable value types (`int?`) and reference types.
- **Best-effort parsing** — convert user-supplied strings to numbers, dates, GUIDs, enums, and more without wrapping every call site in try/catch; get a fallback value back on failure.
- **Per-key locking** — serialize work that must not run concurrently *for the same key* while letting different keys run in parallel, e.g. file writes or cache population keyed by an identifier.
- **Text and HTML cleanup** — truncate at word boundaries, split camelCase identifiers, strip all HTML markup from untrusted input.
- **Task error handling** — fail fast with a single helper when a fire-and-forget or pre-completed `Task<T>` did not complete successfully.
- **Password hashing** — hash and verify passwords with PBKDF2-SHA256 (random salt, fixed-time comparison) without pulling in a dedicated security dependency.

## Usage examples

### Null safety

```csharp
using NTDLS.Helpers;

string? name = GetUserName(); // may return null

// Throws ArgumentNullException if name is null or empty.
string safeName = name.EnsureNotNullOrEmpty();

// Fallback value instead of throwing.
string displayName = name.DefaultWhenNullOrEmpty("<unnamed>");

// Works with nullable value types too.
int? age = GetAge();
int realAge = age.EnsureNotNull("Age is required."); // throws ArgumentException with custom message
int defaultAge = age.DefaultWhenNull(0);             // 0 when null
bool hasAge = age.IsNotDefault();                    // false for null or 0
```

### String-to-type conversion

```csharp
using System.Globalization;
using NTDLS.Helpers;

// Best effort: returns the default value if parsing fails or the input is null.
int port = Converters.ConvertTo<int?>("8080", defaultValue: 80);
int badPort = Converters.ConvertTo<int>("not-a-number", defaultValue: 0); // 0

// Strict: throws FormatException on invalid input.
var guid = Converters.ConvertTo<Guid>("3fa85f64-5717-4562-b3fc-2c963f66afa6");

// Culture-aware parsing of decimal values.
double price = Converters.ConvertTo<double>("1.234,56", culture: CultureInfo.GetCultureInfo("de-DE")); // 1234.56
```

### Per-key locking

```csharp
using NTDLS.Helpers;

var gate = new AtomicKeyOperation();

// Calls with the same key ("user-42") run one at a time...
gate.Execute("user-42", () => SaveUserData(42));

// ...while calls with different keys run concurrently.
await Task.WhenAll(
    Task.Run(() => gate.Execute("user-42", () => SaveUserData(42))),
    Task.Run(() => gate.Execute("user-43", () => SaveUserData(43))));
```

### Text and HTML helpers

```csharp
using NTDLS.Helpers;

// Truncate at a word boundary, adding "..." automatically.
string summary = Text.TruncateAtWord("The quick brown fox jumps over the lazy dog", 20);
// "The quick brown ..."

// Strip all HTML tags and entities from untrusted input.
string plain = HtmlScope.StripHtml("<p>Hello <b>world</b>&amp; friends</p>");
// "Hello world friends"
```

### Task error checking

```csharp
using NTDLS.Helpers;

Task<string> load = LoadConfigAsync(); // started earlier, not yet awaited

// Throws if the task faulted, so errors are not silently swallowed.
Tasks.ThrowTaskException(load);
string config = load.Result;
```

### Password hashing

```csharp
using NTDLS.Helpers;

// Hash with PBKDF2-SHA256 (random salt, 210,000 iterations).
string storedHash = PasswordHashing.Hash("S3cret!");

// Later, verify a candidate password against the stored hash.
bool ok = PasswordHashing.Verify("S3cret!", storedHash);   // true
bool bad = PasswordHashing.Verify("wrong", storedHash);    // false
```

## License

[MIT](https://choosealicense.com/licenses/mit/)
