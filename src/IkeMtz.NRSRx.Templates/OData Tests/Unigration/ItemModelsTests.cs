using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IkeMtz.NRSRx.Core.Models;
using IkeMtz.NRSRx.Core.Unigration;
using NRSRx_ServiceName.Models.V1;
using NRSRx_ServiceName.Data;
using NRSRx_ServiceName.Tests;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace NRSRx_ServiceName.OData.Tests.Unigration
{
  [TestClass]
  public partial class ItemModelsTests : BaseUnigrationTests
  {
    [TestMethod]
    [TestCategory("Unigration")]
    public async Task GetItemModelsTest()
    {
      var itemModel = Factories.ItemModelFactory();
      using var srv = new TestServer(TestHostBuilder<Startup, UnigrationODataTestStartup>()
          .ConfigureTestServices(x =>
          {
            ExecuteOnContext<DatabaseContext>(x, db =>
            {
              _ = db.ItemModels.Add(itemModel);
            });
          })
       );
      var client = srv.CreateClient();
      GenerateAuthHeader(client, GenerateTestToken());

      var resp = await client.GetStringAsync($"odata/v1/{nameof(ItemModel)}s?$count=true");

      //Validate OData Result
      TestContext.WriteLine($"Server Reponse: {resp}");
      var envelope = JsonConvert.DeserializeObject<ODataEnvelope<ItemModel>>(resp);
      Assert.AreEqual(itemModel.Name, envelope?.Value.First().Name);
    }
  }
}