# OBS
### Jag kunde inte pulla docker images from detta repo pga att jag saknade rättigheter, kunde konstigt nog pusha paket. 
![image](https://user-images.githubusercontent.com/58253756/137459858-97db4241-1715-4956-8433-9ec5037f0d67.png)
### Jag fick flytta min deploy av App service delen till ett eget repo som ligger här: https://github.com/bjork-dev/Source (main branch) (All kod är dock samma på detta repo som det, har enbart använt det för deploy och webhooks)

### Azure Functions har deployats from detta repo. 

### Länk till sida: https://bjorkdev-calc-app.azurewebsites.net/

# Diagram
![diagram](https://user-images.githubusercontent.com/58253756/137461942-3bb69fdf-401c-4630-95c6-40b61b7fe746.png)

## Funktion
Vår app service är helt stateless, den enda funktionen sidan har är att presentera användaren med en simpel kalkylator.
![image](https://user-images.githubusercontent.com/58253756/137462791-3424de36-38b2-4e76-be42-c0cbd9ade790.png)

Under huven så hämtar appen de 10 senaste beräkningarna från vår CosmosDB databas vid GET anrop till sidan och POSTar beräkningen när man klickar på "=" tecknet.

## Appens flow är i följande ordning, med kodexempel:

### Användare gör en beräkning, hela kalkylatorn ligger i en POST form, som triggas när SUBMIT knappen klickas på, vilket är "=" knappen => 

```html
<form method="post">
    <table class="calculator">
        <tr>
            <td colspan="3"> <input class="display-box" type="text" id="result" name="result" /> </td>
            <!-- clearScreen() function clear all the values -->
            <td> <input class="button" type="button" value="C" onclick="clearScreen()" style="background-color: #1e90ff;" /> </td>
        </tr>
        <tr>
            ... Har tagit bort lite imellan för att spara plats

            <td> <input class="button" type="submit" value="=" style="background-color: #1e90ff;" /> </td>

        </tr>
    </table>
</form>
```
#### Backend så skickas det vidare till vår DataAccess klass.

```csharp
 public async Task OnPostAsync(string result)
        {
            try
            {
                var response = await _data.Calculate(result);
                
                ....
            }
         }
```

### Appen hämtar Azure function nyckel via KeyVault SDK =>

```csharp
private async Task<string> GetSecret(string secretName)
        {
            var azure = new DefaultAzureCredentialOptions { ExcludeVisualStudioCredential = true };
            var client = new SecretClient(new Uri("https://bjorkdev.vault.azure.net/"), new DefaultAzureCredential(azure), _options);
            KeyVaultSecret secret = await client.GetSecretAsync(secretName);
            return secret.Value;
        }
```

### Appen gör en POST till rätt funktion (beroende på om det "+" eller "-" => 

```csharp
if (numbers.Contains('+')) {
                code = await GetSecret("AddNumbers");
                var addResult = await _httpClient.PostAsync($"https://bjorkdev-calculator.azurewebsites.net/api/AddNumbers?code={code}", data);
                
                if(addResult.IsSuccessStatusCode)
                    return await addResult.Content.ReadAsStringAsync();
                return null;
            }
            code = await GetSecret("SubNumbers");
            var subResult = await _httpClient.PostAsync($"https://bjorkdev-calculator.azurewebsites.net/api/SubNumbers?code={code}", data);
            
            if(subResult.IsSuccessStatusCode)
                return await subResult.Content.ReadAsStringAsync();
            return null;
```
#### Om vår input är fel kommer NULL att returneras, som jag hanterar i detta fall genom att bara uppdatera sidan. (Inga exceptions eller felmeddelanden kastas)

#### Funktionen AddNumbers eller SubNumbers beräknar vår ekvation direkt via DataTable klassen som har en Compute metod som kan beräkna direkt från strängar. Sedan lägger funktionen till beräkningen i databasen via CosmosDB attributen som abstraktherar bort connection, osv. => 

```csharp
if (!data.Contains('+'))
                return new BadRequestObjectResult("Missing '+' operator"); // '-' Om det är subnumbers

            try
            {
                DataTable dt = new DataTable();
                string result = dt.Compute(data, "").ToString();
                log.LogInformation(result);
                await calculation.AddAsync(new Calculation
                {
                    Id = System.Guid.NewGuid().ToString(),
                    Result = $"{data} = {result}",
                    RunDate = DateTime.Now
                });

                return new OkObjectResult(result);
            }
            catch (System.Exception ex)
            {
                log.LogError(ex.Message);
                return new BadRequestObjectResult(ex.Message);
            }
```

### Appen gör en GET request => 

```csharp
public async Task OnGetAsync()
        {
            Results = await _data.GetNumbers();
        }
```
### DataAccess 
```csharp
public async Task<Queue<Calculation>> GetNumbers()
        {
            string code = await GetSecret("GetCalculations");
            var response = await _httpClient.GetAsync($"https://bjorkdev-calculator.azurewebsites.net/api/GetCalculations?code={code}");

            if(!response.IsSuccessStatusCode)
                return null;

            var queue = await response.Content.ReadFromJsonAsync<Queue<Calculation>>();
            return queue;
        }

```


GetCalculations-funktionen hämtar de senaste 10 beräkningarna (rangordnade på datum) och returnerar dem till appen.

```csharp
...
if (calcs == null)
                return new NotFoundObjectResult("No entries found.");

            var latestCalculations = calcs.OrderByDescending(c => c.RunDate).Take(10);
            
            return new OkObjectResult(latestCalculations);
...
```

# Deployment Flow
Diagrammet visar min vår CI/CD pipeline fungerar.

![asdaf drawio](https://user-images.githubusercontent.com/58253756/137464743-a2cd7418-682f-4226-a7fb-3ed2f4e66709.png)

Kod pushas till repot, därefter byggs den och deployas enligt korrekt workflow (Azure functions och app service har varsin)
Om det är functions som har fått commits kommer koden pushas direkt till Azure via Actions, via vår PUBLISH PROFILE som ligger som en secret i detta repo.

![image](https://user-images.githubusercontent.com/58253756/137465030-e792caea-8847-4e6d-be68-fd22b92f36ad.png)

Är det app serice som fått commits kommer en ny docker image att byggas och publiceras i Container Registry, därefter kommer en webhook som ligger konfiugrerad i repot och som pekar mot vår app service att triggas och Azure kommer hämta den senaste imagen och automatiskt restarta vår app service.

![image](https://user-images.githubusercontent.com/58253756/137465313-d9591ee2-816f-4d96-a784-f49317151508.png)

### Azure Deployment
![image](https://user-images.githubusercontent.com/58253756/137465393-67754bc4-cbde-4379-bd2a-5d3433168a38.png)



