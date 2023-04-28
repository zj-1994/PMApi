using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PM_Api.Common;
using PM_Api.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.UI;
using PM_Api.App_Start;
using System.Drawing.Imaging;

//using System.Drawing;

namespace PM_Api.Controllers
{
    public class ServerController : ApiController
    {
        //使用私有对象作为锁，避免死锁
        private object syncObj = new object();



        //http://localhost:61520/api/Server/QueryDate?JsonParm={"selectA1":[{"Name":"156324"}]}
        //http://localhost:61520/api/Server/QueryDate?JsonParm={"selectA3":[{"Name":"%156324%"}]}
        //http://localhost:61520/api/Server/QueryDate?JsonParm={"selectA1":[{"Name":"156324"}],"selectA2":[{"IdA":"11","IdB":"12"},{"IdA":"13","IdB":"14"}]}
        #region 查询数据 -- QueryDate(JsonParm)
        /// <summary>
        /// 查询数据
        /// </summary>
        /// <param name="JsonParm">json对象 </param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("QueryDate")]
        [ActionFilter]
        public IHttpActionResult QueryDate([FromBody] string JsonParm)
        {
            lock (syncObj)
            {
                try
                {
                    DataSet ds = JsonConvert.DeserializeObject<DataSet>(JsonParm);
                    List<SQLParam> SQLParamList = SqlHelp.ParamAdd(ds);
                    var data = SqlHelp.ExecuteQuerySqlTran(SQLParamList);
                    return Ok(JObject.Parse(data));
                }
                catch (Exception ex)
                {
                    return Ok(new { code = 500, msg = ex.Message, error = ex });
                }
            }

        }

        #endregion

        //http://localhost:61520/api/Server/PageDate?JsonParm={"page1":[{"pageindex":0,"pagesize":10,"Name":"779039","Remarks":""}]}
        #region 分页查询 -- PageDate(JsonParm)
        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="JsonParm">json对象 </param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("PageDate")]
        public IHttpActionResult PageDate([FromBody] string JsonParm)
        {
            lock (syncObj)
            {
                try
                {
                    DataSet ds = JsonConvert.DeserializeObject<DataSet>(JsonParm);
                    List<SQLParam> SQLParamList = SqlHelp.ParamAdd(ds);
                    var data = SqlHelp.ExecuteQuerySqlTranPage(SQLParamList);
                    return Ok(JObject.Parse(data));
                }
                catch (Exception ex)
                {
                    return Ok(new { code = 500, msg = ex.Message, error = ex });
                }
            }
        }

        #endregion

        //http://localhost:61520/api/Server/OperationDate?JsonParm={"insertB":[{"Name":"111","Remarks":"222"},{"Name":"333","Remarks":"444"},{"Name":"555","Remarks":"666"}]}
        #region 增删改数据 -- OperationDate(JsonParm)
        /// <summary>
        /// 增删改数据
        /// </summary>
        /// <param name="JsonParm">json对象 </param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("OperationDate")]
        public IHttpActionResult OperationDate([FromBody] string JsonParm)
        {
            lock (syncObj)
            {
                try
                {
                    DataSet ds = JsonConvert.DeserializeObject<DataSet>(JsonParm);
                    List<SQLParam> SQLParamList = SqlHelp.ParamAdd(ds);
                    var data = SqlHelp.ExecuteOperationSqlTran(SQLParamList);
                    return Ok(JObject.Parse(data));
                }
                catch (Exception ex)
                {
                    return Ok(new { code = 500, msg = ex.Message, error = ex });
                }
            }
        }

        #endregion

        //http://localhost:61520/api/Server/ExportExcel?JsonParm={"selectA1":[{"Name":"156324"}],"selectA2":[{"IdA":"11","IdB":"12"},{"IdA":"13","IdB":"14"}]}
        #region 导出Excel -- ExportExcel(JsonParm)
        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="JsonParm">json对象 </param>
        /// <returns></returns>
        [HttpGet]
        [ActionName("ExportExcel")]
        public void ExportExcel([FromUri] string JsonParm)
        {
            lock (syncObj)
            {
                try
                {
                    DataSet ds = JsonConvert.DeserializeObject<DataSet>(JsonParm);
                    List<SqlParameter> paramList = new List<SqlParameter>();
                    string TableName = ds.Tables[0].TableName;
                    string sql = SqlHelp.GetSql(TableName);
                    for (int j = 0; j < ds.Tables[0].Rows.Count; j++)
                    {
                        for (int k = 0; k < ds.Tables[0].Columns.Count; k++)
                        {
                            var key = ds.Tables[0].Columns[k].ToString();
                            var value = ds.Tables[0].Rows[j]["" + key + ""].ToString();
                            SqlParameter param = new SqlParameter("@" + key, value);
                            paramList.Add(param);
                        }
                    }
                    var data = SqlHelp.SqlGetDataTable(sql, paramList.ToArray());
                    var uuid = Guid.NewGuid().ToString();
                    string[] strArray = uuid.Split('-');
                    ExcelHelp.ExportByWeb(data, DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-") + strArray[4] + ".xls");
                }
                catch (Exception ex)
                {
                    Console.Write("导出错误: {0}", ex.Message);
                    throw;
                }
            }
        }

        #endregion

        #region 导入Excel -- ImportExcel(JsonParm)
        /// <summary>
        /// 导入Excel
        /// </summary>
        /// <param name="TableName">表名</param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("ImportExcel")]
        public dynamic ImportExcel()
        {
            lock (syncObj)
            {
                try
                {
                    var request = System.Web.HttpContext.Current.Request;
                    if (request.Files.Count > 0)
                    {
                        var file = request.Files[0];
                        var TableName = request.Form["TableName"];
                        var x = file.FileName;
                        string Folder = "/ImportExcel/";
                        string path = HttpContext.Current.Server.MapPath(Folder);
                        //检查是否存在文件夹
                        if (!Directory.Exists(path))
                        {
                            //创建文件夹
                            Directory.CreateDirectory(path);
                        }
                        Random ran = new Random();
                        string RandKey = ran.Next(100000, 999999).ToString();
                        file.SaveAs(path + Path.GetFileNameWithoutExtension(file.FileName) + "_" + RandKey + System.IO.Path.GetExtension(file.FileName));
                        string filename = path + Path.GetFileNameWithoutExtension(file.FileName) + "_" + RandKey + System.IO.Path.GetExtension(file.FileName);
                        var dt = ExcelHelp.Import(filename);
                        ExcelHelp.InsertTable(dt, TableName);
                        File.Delete(filename);
                        return new { code = 200, msg = "导入完成" };
                    }
                    else
                    {
                        return new { code = 500, msg = "请选择导入文件！" };
                    }
                }
                catch (Exception ex)
                {
                    return Ok(new { code = 500, msg = ex.Message, error = ex });
                }
            }
        }

        #endregion

        #region 生成二维码
        /// <summary>
        /// 生成二维码
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("QRcode")]
        public IHttpActionResult QRcode([FromBody] string code)
        {
            lock (syncObj)
            {
                try
                {
                    string base64String = "";
                    QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.M);
                    QrCode qrCode = qrEncoder.Encode(code);
                    GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(5, QuietZoneModules.Two), Brushes.Black, Brushes.White);
                    var size = renderer.SizeCalculator.GetSize(qrCode.Matrix.Width);
                    Bitmap image = new Bitmap(size.CodeWidth, size.CodeWidth);
                    //renderer.Draw(image.CreateGraphics(), qrCode.Matrix);
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        renderer.Draw(g, qrCode.Matrix);
                    }
                    using (MemoryStream ms = new MemoryStream())
                    {
                        // 将图片保存到内存流中
                        image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] imageBytes = ms.ToArray();

                        // 将byte数组转换成base64编码字符串
                       base64String = Convert.ToBase64String(imageBytes);
                    }
                    return Ok(new { code = 200, msg = "ok", data = base64String });
                }
                catch (Exception ex)
                {
                    return Ok(new { code = 500, msg = ex.Message, error = ex });
                }
            }
        }

        #endregion

        #region 验证码

        [HttpPost]
        public IHttpActionResult VerificationCode()
        {
            try
            {
                //画布大小
                int width = 100;
                int height = 50;

                // 随机字符串
                Random random = new Random((int)DateTime.Now.Ticks);
                string code = "";
                for (int i = 0; i < 4; i++)
                {
                    int num = random.Next(0, 10);
                    code += num.ToString();
                }

                // 创建bitmap对象并绘制
                Bitmap bitmap = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                Graphics graphics = Graphics.FromImage(bitmap);

                // 将图片背景填充成白色
                graphics.Clear(Color.White);

                // 添加随机字符
                Font font = new Font("Arial", 24, FontStyle.Bold | FontStyle.Italic);
                Brush brush = new SolidBrush(Color.Black);
                graphics.DrawString(code, font, brush, new PointF(15, 10));

                // 绘制验证码噪点
                for (int i = 0; i < random.Next(60, 80); i++)
                {
                    int pointX = random.Next(bitmap.Width);
                    int pointY = random.Next(bitmap.Height);
                    graphics.FillEllipse(new SolidBrush(Color.FromArgb(random.Next(256), random.Next(256), random.Next(256))), pointX, pointY, 3, 3); // 绘制半径为 5 的圆
                }


                // 添加干扰线
                for (int i = 0; i <= 5; i++)
                {
                    Pen pen = new Pen(Color.FromArgb(random.Next(0, 255), random.Next(0, 255), random.Next(0, 255)), 3);
                    graphics.DrawLine(pen, random.Next(0, width), random.Next(0, height), random.Next(0, width), random.Next(0, height));
                }

                // 将bitmap转化为字节数组
                MemoryStream stream = new MemoryStream();
                bitmap.Save(stream, ImageFormat.Jpeg);
                byte[] bytes = stream.ToArray();

                // 清理资源
                graphics.Dispose();
                bitmap.Dispose();

                var jpg = Convert.ToBase64String(bytes);
                return Ok(new { Code = 200, Msg = "请求成功！", Data = new { VerificationCode = code, Base64jpg = jpg } });

            }
            catch (Exception ex)
            {
                return Ok(new { Code = 500, Msg = ex.Message });
            }
        }

        #endregion

        #region 获取系统日志
        /// <summary>
        /// 获取系统日志
        /// </summary>
        /// <param name="myString"></param>
        /// <returns></returns>
        [HttpPost]
        public IHttpActionResult LogInfo([FromBody] string JsonParm)
        {
            lock (syncObj)
            {
                try
                {
                    DataTable dt = JsonConvert.DeserializeObject<DataTable>(JsonParm);
                    Dictionary<string, string> dict = dt.Columns.Cast<DataColumn>().ToDictionary(c => c.ColumnName,c => dt.Rows[0][c].ToString());
                    string sql = "select * from(SELECT row_number()over(order by a.ID DESC) as iid,* FROM log as a) AS a where a.iid > ((@pageindex-1)*@pagesize) and a.iid <= (@pageindex*@pagesize); SELECT count(*) FROM Log;";
                    var data = SqlHelp.DataSetToJson_Page(SQLite.ExecuteDataset(sql, dict));
                    return Ok(JObject.Parse(data));
                }
                catch (Exception ex)
                {
                    return Ok(new { code = 500, msg = ex.Message, error = ex });
                }
            }
        }

        #endregion

        [HttpPost]
        [ActionFilter("订单管理", "新增", "订单新增功能")]
        public IHttpActionResult Test([FromBody] string JsonParm)
        {
            try
            {
                int a = 10, b = 0;
                int c = a / b;
                return Ok(new { code = 200, msg = "ok" });
            }
            catch (Exception ex)
            {
                return Ok(new { code = 500, msg = ex.Message, error = ex});
            }
            
        }

    }
}
