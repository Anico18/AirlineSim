# AirlineSim
Simulation test for seat assignment in example flights.

EJERCICIO DE POSTULACIÓN: SIMULACIÓN DE CHECK-IN AEROLÍNEA.

El presente ejercicio se realizó haciendo uso del lenguaje de programación C#, haciendo uso de Entity Framework en ASP.NET6. 
Se aplicó Pomelo Entity Framework Core para poder establecer la conexión la base de datos en MySQL.

CONVERSIÓN DE SNAKE CASE A CAMEL CASE

A partir de la creación de los modelos en Entity Framework, se convirtieron los strings de Snake Case a Camel Case, esto se puede
observar con mayor detalle en DBEFContext.cs. A continuación se muestra un fragmento de código de dicho archivo:

    modelBuilder.Entity<Flight>(entity =>
    {
        entity.ToTable("flight");

        entity.Property(e => e.FlightId)
            .ValueGeneratedNever()
            .HasColumnName("flight_id");

        entity.Property(e => e.AirplaneId).HasColumnName("airplane_id");

        entity.Property(e => e.LandingAirport)
            .HasMaxLength(255)
            .HasColumnName("landing_airport");

        entity.Property(e => e.LandingDateTime).HasColumnName("landing_date_time");

        entity.Property(e => e.TakeoffAirport)
            .HasMaxLength(255)
            .HasColumnName("takeoff_airport");

        entity.Property(e => e.TakeoffDateTime).HasColumnName("takeoff_date_time");
    });

En el snippet anterior, se puede observar la creación del modelo 'Flight', en el cual se declaran las propiedades de dicha entidad
con nombres en CamelCase y, en el método `HasColumnName`, se indica el nombre de la columna en Snake Case dentro de la base de datos.
De esta manera, se ha convertido satisfactoriamente los nombres de Snake Case a CamelCase.
  
 FORMATO DE RESPUESTA
  
  En la carpeta de 'Responses', está el archivo `SimAirlineResult.cs`. Este contiene todos los objetos que componen a la respuesta 
  que debe dar la API al momento de consultar. Se declaran los nombres como van a mostrarse en la estructura de respuesta, asegurando
  la similitud entre el ejercicio propuesto y la solución.
  
    public class ResponseData
      {
          public ResponseData() { 
              passengers = new List<ResponsePassenger>();
          }

          public int flightId { get; set; }
          public int? takeoffDateTime { get; set; }
          public string takeoffAirport { get; set; }
          public int? landingDateTime { get; set; }
          public string landingAirport { get; set; }
          public int? airplaneId { get; set; }
          public List<ResponsePassenger> passengers { get; set; }
      }
  
  En el fragmento de código anterior, se observa la clase principal de la respuesta. La parte más importante de este fragmento está en
  la lista `List<ResponsePassenger>`, la cual va a contener la lista de todos pasajeros con sus respectivos datos.
  
  CONTROLADOR DE API
  
  En la carpeta 'Controllers' está el archivo `AirlineSimController.cs`. Este contiene toda la lógica detrás de la solución implementada.
  Las partes fuertes de esta solución están en:
  
    a). Validaciones de nulidad: Se han implentado numerosas validaciones de nulidad, para que se eviten los errores a causa de datos que 
                                 podrían resultar nulos. Tienen el formato: `if(variable != null){}`
  
    b). Asignación de asientos por categoría: Se busca que los asientos sean asignados específicamente a la categoría indicada en los
                                 boarding passes. 
  
    c). Priorización de asientos escogidos: Algunos boarding passes tienen asientos asignados, dentro de la solución se hace la validación
                                 de esto y se asignan los asientos que indica el boarding pass.
  
    d). Agrupación de compra: Existen compras de varios boarding passes en grupo. Para impedir separaciones, se mantiene y asigna asientos
                                 contiguos para los grupos de personas en cuyos boarding passes tienen el mismo `purchase_id`. Esto se logra
                                 validando si el boarding pass tiene un `purchase_id` con uno o varios boarding passes. Si se diese el caso
                                 de toparse con una compra de numerosos boarding passes, se asigna asientos a todos los boarding passes con
                                 ese `purchase_id`. Es, también, a partir de esto, que se puede mantener a menores de edad contiguo a una
                                 persona mayor de edad del mismo grupo de boarding passes.
  
  ESTRUCTURA DE LA SOLUCIÓN.
  
  a). Creación de variables principales:
  
  
      var boarding = await _context.BoardingPasses.ToListAsync();
                var purchase = await _context.Purchases.ToListAsync();
                var seat = await _context.Seats.ToListAsync();
                var iterationFc = 0;
                var iterationPe = 0;
                var iterationE = 0;
                var emergencyiterationFc = 0;
                var emergencyiterationPe = 0;
                var emergencyiterationE = 0;

                var queryboarding = await _context.BoardingPasses
                    .Include(m => m.Passenger)
                    .Include(m => m.Purchase)
                    .Include(m => m.Flight)
                    .ThenInclude(m => m.Airplanes)
                    .ThenInclude(m => m.Seats)
                    .Where(m => m.FlightId == flyRequest.id)
                    .ToListAsync();

                var flightData = _context.Flights.FirstOrDefault(m => m.FlightId == flyRequest.id);
                var economyClass = new List<Seat>();

  
  La variable `boarding` contiene una lista de todos los BoardingPasses, la variable `purchase` contiene la lista de todas las compras y la
  variable `seat` enlista todos los asientos en la base de datos. Las variables `iterationFc`, `iterationPe`, `iterationE`, `emergencyIterationFc`,
  `emergencyIterationPe` y `emergencyIterationE`; las cuales están inicializadas en 0, son contadores que se utilizarán más adelante durante la
  asignación de asientos. En la variable `queryboarding` se enlistan sólo los boarding passes del vuelo del que se está haciendo la request; esta 
  variable incluye al resto de entidades relacionadas a 'BoardingPasses'. En `flightData` aparecerá la información del vuelo requerido y `economyClass`
  es una lista vacía de la clase 'Seat'.
  
  b). Creación de listas para iterar:
  
    var firstClass = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId && m.SeatTypeId == 1).ToListAsync();
                var emergencyFc = firstClass.OrderByDescending(m => m.SeatId).ToList();
                var premiumEconomy = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId && m.SeatTypeId == 2).ToListAsync();
                var emergencyPe = premiumEconomy.OrderByDescending(m => m.SeatId).ToList();
                var firstEconomySeat = await _context.Seats.FirstOrDefaultAsync(m => m.SeatTypeId == 3 && m.AirplaneId == flightData.AirplaneId);
                if(firstEconomySeat != null)
                {
                    var seatcolumns = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId).Select(m => m.SeatColumn).Distinct().ToListAsync();
                    foreach(var seatColumn in seatcolumns)
                    {
                        var economyCol = await _context.Seats.Where(m => m.AirplaneId == flightData.AirplaneId && m.SeatColumn == seatColumn && m.SeatRow >= firstEconomySeat.SeatRow).ToListAsync();
                        economyClass.AddRange(economyCol);
                    }
                    var emergencyE = economyClass.OrderByDescending(m => m.SeatId).ToList();
                    List<int> processedBoardingPasses = new List<int>();
                    ResponseFly responseFly = new ResponseFly();
                    ResponseData responseData = new ResponseData();
  
  
  Las variables `firstClass`, `premiumEconomy` y `economyClass` contienen las listas de asientos de cada una de las clases. Por algún motivo, al querer
  formar `economyClass` de manera análoga a las otras dos variables, se obtenía una lista de 9 elementos. Es por eso que se decidió formar la lista a 
  partir del `foreach(var seatColumn in seatcolumns)`, en el cual se utiliza una lista de columnas de asientos guardada en `seatcolumns`. Esta manera de
  obtener las columnas y las filas (en `economyCol`) asegura que el código sea mantenible y escalable a lo largo del tiempo. Las variables `emergencyFc`,
  `emergencyPe` y `emergencyE` son las listas de asientos invertidas, las cuales se utilizarán para manejar posibles asignaciones de asientos fallidas que
  terminen en `null`. Se crea también la lista de `processedBoardingPasses`, a la cual se les agregarán los boarding passes que ya hallan sido procesados.
  En las últimas dos línea se crean los objetos que se utilizarán para contener la respuesta.
  
  c). Asignación de asientos
  
  
      if (flightData != null) {
              var airplaneInUse = flightData.AirplaneId;
              var firstSeatId = _context.Seats.FirstOrDefault(m => m.AirplaneId == airplaneInUse);
              if (firstSeatId != null)
              {
                  int seatIncremental = firstSeatId.SeatId;
                  if (queryboarding != null)
                  {
                      foreach (var item in queryboarding)
                      {
                          if(item.SeatId != null)
                          {
                              if (item.Purchase != null)
                              {
                                  if (item.Purchase.BoardingPasses.Count == 1)
                                  {
                                      if (processedBoardingPasses.Contains(item.BoardingPassId))
                                      {
                                      }
                                      else
                                      {
                                          if (item.Passenger != null)
                                          {
                                              if (item.SeatTypeId == 1)
                                              {
                                                  item.SeatId = item.SeatId;

                                                  if (item.SeatId == null)
                                                  {
                                                      item.SeatId = emergencyFc[emergencyiterationFc].SeatId;
                                                      emergencyiterationFc++;
                                                  }

                                                  ResponsePassenger responsePassenger = new ResponsePassenger();
                                                  responsePassenger.passengerId = item.Passenger.PassengerId;
                                                  responsePassenger.dni = item.Passenger.Dni ?? "";
                                                  responsePassenger.name = item.Passenger.Name ?? "";
                                                  responsePassenger.age = item.Passenger.Age;
                                                  responsePassenger.country = item.Passenger.Country ?? "";
                                                  responsePassenger.boardingPassId = item.BoardingPassId;
                                                  responsePassenger.purchaseId = item.PurchaseId;
                                                  responsePassenger.seatTypeId = item.SeatTypeId;
                                                  responsePassenger.seatId = item.SeatId;

                                                  responseData.passengers.Add(responsePassenger);
                                                  processedBoardingPasses.Add(item.BoardingPassId);
                                                  Seat seatToRemoveFc = firstClass.FirstOrDefault(m => m.SeatId == item.SeatId);
                                                  if (seatToRemoveFc != null)
                                                  {
                                                      firstClass.Remove(seatToRemoveFc);
                                                  }
                                              }

  
  La variable `airplaneInUse` contiene la información del avión utilizado en el viaje del request. Esta información permite indicarle a la variable
  `firstSeatId` cuál es el 'Id' del primer asiento del vuelo (para que no empiece siempre en el Id=1) y se asigna a la variable `seatIncremental`.
  `foreach (var item in queryboarding)` inicia la revisión de todos los boarding passes del vuelo y, luego d se procesa la condición 
  `if(item.SeatId != null)` con el fin de filtrar si tiene o no un asiento ya asignado. Si no lo tiene, continua con de validaciones de no nulidad
  y la condición `if (item.Purchase.BoardingPasses.Count == 1)`, la cual busca filtrar a los grupos de boarding passes. Si es sólo un boarding pass
  en la compra, continúa con la condición `if (processedBoardingPasses.Contains(item.BoardingPassId))`; esta va a saltarse el boarding pass en caso
  exista en la lista de procesados. Luego, el algoritmo continúa con la condición   `if (item.SeatTypeId == 1)`, la cual busca filtrar la clase a 
  la que pertenece el boarding pass. 
  En caso `SeatTypeId != 1`, existen las otras dos opciones con validaciones y estructuras similares a este snippet. Si corresponde a la clase, se asigna
  el asiento y se verifica si la asignación fue satisfactoria en `if (item.SeatId == null)`. Si no es satisfactoria, se llama a la lista de emergencia, 
  se asigna el último asiento de la clase (el primero de esa lista) y se le suma 1 al incremental de emergencia. Posteriormente, se asigan los valores
  al objeto de respuesta en las líneas con `responsePassenger` y `responseData`. Para terminar, se añade el boarding pass procesado a la lista de 
  procesados y se remueve el asiento de la lista de disponibles.
  
  
      foreach (var obj in item.Purchase.BoardingPasses) {
              if (processedBoardingPasses.Contains(obj.BoardingPassId))
              {
              }
              else
              {
                  if (item.Passenger != null)
                  {   
                      if (obj.SeatTypeId == 1)
                      {
                          obj.SeatId = obj.SeatId;

                          if (obj.SeatId == null)
                          {
                              obj.SeatId = emergencyFc[emergencyiterationFc].SeatId;
                              emergencyiterationFc++;
                          }
                          ResponsePassenger responsePassenger = new ResponsePassenger();
                          responsePassenger.passengerId = obj.Passenger.PassengerId;
                          responsePassenger.dni = obj.Passenger.Dni ?? "";
                          responsePassenger.name = obj.Passenger.Name ?? "";
                          responsePassenger.age = obj.Passenger.Age;
                          responsePassenger.country = obj.Passenger.Country ?? "";
                          responsePassenger.boardingPassId = obj.BoardingPassId;
                          responsePassenger.purchaseId = obj.PurchaseId;
                          responsePassenger.seatTypeId = obj.SeatTypeId;
                          responsePassenger.seatId = obj.SeatId;

                          responseData.passengers.Add(responsePassenger);
                          processedBoardingPasses.Add(obj.BoardingPassId);
                          Seat seatToRemoveFc = firstClass.FirstOrDefault(m => m.SeatId == obj.SeatId);
                          if (seatToRemoveFc != null)
                          {
                              firstClass.Remove(seatToRemoveFc);
                          }

  
  En caso sean varios boarding passes con el mismo `purchase_id`, se maneja el mismo proceso, pero con `foreach (var obj in item.Purchase.BoardingPasses)`
  previo a todo, haciendo que se procesen todos los boarding passes en ese mismo grupo. Se agregan los pasajeros a la respuesta y se remueven los asientos
  utilizados. 
  
  ALCANCES Y CONSIDERACIONES
  
  Este método busca mantener a la persona en el asiento que ha escogido (en caso lo haya hecho) y procesa los grupos de personas que tiene boarding passes
  con el mismo `purchase_id` juntos, asignándoles asientos contiguos. Esto hace que se pueda mantener a los menores en compañía de un adulto de su mismo
  grupo. Asimismo, se manejan los nulos asignando asientos de la parte de atrás de cada una de las cabinas de cada clase, permitiendo que no exista pasajeros
  con `SeatId` con valor `null`.  
  
  
  
