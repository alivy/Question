using System.Web;
using System.Web.Optimization;

namespace QuestionSystem
{
    public class BundleConfig
    {
        // 有关捆绑的详细信息，请访问 https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // 使用要用于开发和学习的 Modernizr 的开发版本。然后，当你做好
            // 生产准备就绪，请使用 https://modernizr.com 上的生成工具仅选择所需的测试。
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));



            //easyui的layout模板样式
            bundles.Add(new StyleBundle("~/Content/easylayoutcss").Include(
                      "~/Content/easyui/themes/default/easyui.css",
                      "~/Content/easyui/themes/icon.css"
                      ));

            //easyui的外部拓展样式
            bundles.Add(new StyleBundle("~/Content/expandcss").Include(
                     "~/Content/easyui/expandcss/sidemenu_style.css"
                     ));


            //默认jq
            bundles.Add(new ScriptBundle("~/Content/defaultjq").Include(
                      "~/Content/easyui/jquery.min.js"));

            //easyui的模板jq
            bundles.Add(new ScriptBundle("~/Content/easylayoutjq").Include(
                     "~/Content/easyui/jquery.easyui.min.js"));
        }
    }
}
