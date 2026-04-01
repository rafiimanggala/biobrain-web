using CustomControls;
using Microsoft.Maui.Controls;
using Device = Microsoft.Maui.Controls.Device;

namespace BioBrain.AppResources
{
    public class CustomCssStyles
    {
        public static string GlossaryPopupStyle =>
            $@"
<meta charset=""UTF-8"">
{(Device.RuntimePlatform == Device.iOS ? "<meta name=\"viewport\" content=\"initial-scale=1.0\" />" : string.Empty)}
                    <style>

@font-face {{
    font-family: DistroII-Bats;
    src: url(Distro2Bats.ttf);
}}

@font-face {{
    font-family: Nunito;
    src: url(Nunito-Bold.ttf);
}}

@font-face {{
    font-family: 'Roboto-Light';
    src: url(Roboto-Light.ttf);
}}

html {{
                       
    font-family: Nunito;
    font-size: 12pt;
    color: {CustomColors.DarkMainString};
    
    -webkit-touch-callout: none; /* iOS Safari */
    -webkit-user-select: none; /* Safari */
     -khtml-user-select: none; /* Konqueror HTML */
       -moz-user-select: none; /* Firefox */
        -ms-user-select: none; /* Internet Explorer/Edge */
            user-select: none; /* Non-prefixed version, currently
                                  supported by Chrome and Opera */
}}

.termin{{
    color: {CustomColors.DarkMainString};
    font-weight: bold;    
    font-size: 13pt;
}}

article{{
    color: {CustomColors.DarkMainString};
    text-align: left;
    margin:0;
}}

a{{
    color: {CustomColors.DarkMainString};
}}

img{{
    max-width: 100%;
}}

table {{
    border-collapse: collapse;
    width: 95%;
    margin-bottom: 10;
    font-family: 'Roboto-Light';
}}

table, th, td {{
    border: 2px solid {CustomColors.DarkMainString};
	padding:5;
}}

td{{
width: 49%;
}}

.ql-indent-1{{
    margin-left: 16px;
}}
.katex {{ font-family: 'Roboto-Light' !important; font-size: 12pt !important; }}
</style>
<script src=""auto-render.min.js""></script>
<script src=""katex.min.js""></script>
<link rel=""stylesheet"" href=""katex.min.css"">
            ";

        public static string AnswerPopupStyle =>
            $@"
<meta charset=""UTF-8"">
{(Device.RuntimePlatform == Device.iOS ? "<meta name=\"viewport\" content=\"initial-scale=1.0\" />" : string.Empty)}
<style>

@font-face {{
    font-family: DistroII-Bats;
    src: url(Distro2Bats.ttf);
}}

@font-face {{
    font-family: Nunito;
    src: url(Nunito-Bold.ttf);
}}

@font-face {{
    font-family: 'Roboto-Light';
    src: url(Roboto-Light.ttf);
}}

html{{
    color: {CustomColors.DarkMainString};
    font-family: Nunito;
    font-size: 12pt;

    -webkit-touch-callout: none; /* iOS Safari */
    -webkit-user-select: none; /* Safari */
     -khtml-user-select: none; /* Konqueror HTML */
       -moz-user-select: none; /* Firefox */
        -ms-user-select: none; /* Internet Explorer/Edge */
            user-select: none; /* Non-prefixed version, currently
                                  supported by Chrome and Opera */
}}
a{{
    color: {CustomColors.DarkMainString};
}}
img{{
    max-width: 100%;
}}

.correctString{{
    font-size: 16pt !important;
}}

table {{
    border-collapse: collapse;
	color:{CustomColors.DarkMainString};
	font-size: 12pt;
    width: 95%;
    max-width:100%;
    margin-bottom: 10;
}}

table, th, td {{
    border: 2px solid {CustomColors.DarkMainString};
	padding:5;
}}
.ql-indent-1{{
    margin-left: 16px;
}}
.katex {{ font-family: 'Roboto-Light' !important; font-size: 12pt !important; }}
</style>
<script src=""auto-render.min.js""></script>
<script src=""katex.min.js""></script>
<link rel=""stylesheet"" href=""katex.min.css"">
            ";

        public static string MaterialsStyles =>
                    $@"
<!DOCTYPE html>
<meta charset=""UTF-8"">
{(Device.RuntimePlatform == Device.iOS ? "<meta name=\"viewport\" content=\"initial-scale=1.0\" />" : string.Empty)}
<meta name=""format-detection"" content=""telephone=no"">
<style>
    
html{{    
    -webkit-text-size-adjust: 100%;

    -webkit-touch-callout: none; /* iOS Safari */
    -webkit-user-select: none; /* Safari */
     -khtml-user-select: none; /* Konqueror HTML */
       -moz-user-select: none; /* Firefox */
        -ms-user-select: none; /* Internet Explorer/Edge */
            user-select: none; /* Non-prefixed version, currently
                                  supported by Chrome and Opera */
}}
                    
summary::-webkit-details-marker {{
    display: none;
}}

    details details summary::-webkit-details-marker {{
    display: none;
}}

@font-face {{
    font-family: DistroII-Bats;
    src: url(Distro2Bats.ttf);
}}

@font-face {{
    font-family: Nunito;
    src: url(Nunito-Bold.ttf);
}}

@font-face {{
    font-family: 'Roboto-Light';
    src: url(Roboto-Light.ttf) format('truetype');
}}

@font-face {{
    font-family: 'Roboto-Bold';
    src: url(Roboto-Bold.ttf) format('truetype');
}}

b{{
    font-family: 'Roboto-Bold';
    font-weight: normal;
}}

strong{{
    font-family: 'Roboto-Bold';
    font-weight: normal;
}}

summary:before {{ 
    content: "",""; 
    font-family: 'DistroII-Bats';
    font-size: 13pt;
    margin: -5px 5px 0 0; 
    padding: 0; 
    text-align: center; 
    width: 20px;
    color: {CustomColors.LightMainString};
}}

details[open] summary:before {{
    content: "">"";
}}

summary::selection{{
	background: transparent;
}}

summary{{
	color: {CustomColors.DarkMainString};
    font-family: Nunito;
    font-size: 13pt;
    margin-bottom: 10px;
	list-style: none;
}}

summary>span{{
    padding-left:20px;
    -webkit-box-decoration-break: clone;
    box-decoration-break: clone;
}}
summary>span>span{{
    margin-left:-20px;
}}

details{{
	color: {CustomColors.DarkMainString};
    font-size: 12pt;
    font-family: 'Roboto-Light';
	margin-right: 7px;
    margin-left: 7px;
    max-width: 100%;
}}

details[open]{{
	border-bottom: 1px solid {CustomColors.LightMainString};
}}

summary:focus {{
	outline-style: none;
}}
article > details > summary {{
	margin-top: 20px;
}}
details > p {{
    margin-top: 0px;
    margin-bottom: 10px;
}}

sup {{
    font-size: 70%;
    position: relative;
    bottom: 0.9ex;
}}

ol {{
    Margin-top: 0pt;
    margin-bottom: 10px;
}}

ul {{
    Margin-top: 0pt;
    margin-bottom: 10px;
}}

li{{
    margin-bottom: 5px;
}}

li>ul{{
  list-style-type: initial;
}}

a{{
    color:{CustomColors.DarkMainString};
}}

table {{
    border-collapse: collapse;
	font-size: 12pt;
    color:{CustomColors.DarkMainString};
    margin-left: auto;
    margin-right: auto;
    margin-bottom: 10px;
    max-width: 100%;
}}

table, th, td {{
    border: 2px solid {CustomColors.DarkMainString};
	padding:5;
    word-wrap: break-word;
}}

th{{
    font-family: 'Roboto-Bold';
    font-weight: normal;
}}

td{{
    vertical-align: top;
}}

td > ul{{
    padding-left: 18px;
}}

td > ol{{
    padding-left: 18px;
}}

td.diagonalCross
{{
	position:   relative;
	background: linear-gradient(to bottom right,  transparent calc(50% - 1px), red, transparent calc(50% + 1px));
}}

td.diagonalCross:before
{{
	content:     "";
	display:     block;
	position:    absolute;
	width:       100%;
	height:      100%;
	top:         0;
	left:        0;
	z-index:     -1;
	background:  linear-gradient(to top right, transparent calc(50% - 1px), red, transparent calc(50% + 1px));
}}

img{{
    display: block;
    max-width: 100%;
    margin-bottom: 15px;
    margin-top: 5px;
}}

.ql-indent-1{{
    margin-left: 16px;
}}

.katex {{ font-family: 'Roboto-Light' !important; font-size: 12pt !important; }}

</style>
<script src=""auto-render.min.js""></script>
<script src=""katex.min.js""></script>
<link rel=""stylesheet"" href=""katex.min.css"">
</script>
";

        /*        
<script defer src=""https://cdn.jsdelivr.net/npm/katex@0.11.1/dist/contrib/auto-render.min.js"" integrity=""sha384-kWPLUVMOks5AQFrykwIup5lo0m3iMkkHrD0uJ4H5cjeGihAutqP0yW0J6dpFiVkI"" crossorigin=""anonymous""></script>
<script defer src=""https://cdn.jsdelivr.net/npm/katex@0.11.1/dist/katex.min.js"" integrity=""sha384-y23I5Q6l+B6vatafAwxRu/0oK/79VlbSz7Q9aiSZUvyWYIYsd+qj+o24G5ZU2zJz"" crossorigin=""anonymous"" ></script>
<link rel=""stylesheet"" href=""https://cdn.jsdelivr.net/npm/katex@0.11.1/dist/katex.min.css"" integrity=""sha384-zB1R0rpPzHqg7Kpt0Aljp8JPLqbXI3bhnPWROx27a9N0Ll6ZP/+DiW/UqRcLbRjq"" crossorigin=""anonymous"">

<script id=""MathJax-script"" async
        src=""https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-chtml.js"">
        </script>
<script>
window.onload=function(){{
window.MathJax.Hub.Queue([""Typeset"", window.MathJax.Hub, document.body]);
}};
window.onchange=function(){{
    window.renderMathInElement(document.body);
}};
window.onclick=function(){{
window.MathJax.Hub.Queue([""Typeset"", window.MathJax.Hub, document.body]);
}};
         */

        public static string QuestionsStyles =>
            $@"
<meta charset=""UTF-8"">
<meta name=""format-detection"" content=""telephone=no"">
{(Device.RuntimePlatform == Device.iOS ? "<meta name=\"viewport\" content=\"initial-scale=1.0\" />" : string.Empty)}
<style>

@font-face {{
    font-family: DistroII-Bats;
    src: url(Distro2Bats.ttf);
}}

@font-face {{
    font-family: Nunito;
    src: url(Nunito-Bold.ttf);
}}

@font-face {{
    font-family: 'Roboto-Light';
    src: url(Roboto-Light.ttf);
}}

button{{
    clear:both;
    width: 100%;
margin-right: 7px;
	padding     : 0;
	border      : 0;
  background: {CustomColors.DarkMainString};
	height: auto;
  display: inline-table;
 font-size: 12pt !important;
}}

.letter{{
padding:0;
  text-align: center;
 background:white;
 color: {CustomColors.DarkMainString};
 width:40; 
height:40;
 display: table-cell;
 vertical-align:middle;
 font-family: Nunito;
 font-size: 12pt;
}}

button > span{{
text-align: left;
 background-color: {CustomColors.DarkMainString};
 white-space: normal;
 vertical-align:middle; 
 margin:10;
 padding-left: 10; 
 padding-right: 10;
 font-family: Nunito;
 font-size: 12pt !important;
 color: white;
 display: table-cell;
font-weight: 500;
}}

.leftPair{{
display: table-cell;
  width:100%;
}}

.rightPair{{
display: table-cell;
  width:100%;
}}

.leftPair>.buttonLine{{
margin-right: 2.5px;
}}

.rightPair>.buttonLine{{
margin-left: 2.5px;
}}

.container{{
  display: inline-table;
  width:100%;
}}

.test{{
background: {CustomColors.LightMainString};
height:100%;
}}

.questionText{{
color:white;
font-family: Nunito;
font-size: 13pt;
margin-bottom:25px;
margin-top: 15;
margin-right: 7px;
margin-left: 7px;
}}

.questionNumber {{
float: left;
margin-bottom:15px;
}}

.questionBody{{
clear: both;
margin-top: 15px;
}}

.levelName {{
text-align: center;
position: relative;
right: 25%;
width: 50%;
float: right;
margin-bottom:15px;
}}

p{{
    color: white;
font-family: 'Roboto-Light';
font-size: 12pt;
text-align: left;
margin-bottom: 20;
margin-right: 7px;
margin-left: 7px;
}}

li{{
    color: white;
font-family: 'Roboto-Light';
font-size: 12pt;
text-align: left;
margin-bottom: 20;
margin-right: 7px;
margin-left: 7px;
}}

.correctString{{
 font-family: Nunito;
 font-size: 16pt;
 color: {CustomColors.DarkMainString};
}}

.correctBlock{{
 border: 4px solid {CustomColors.DarkMainString};
 border-radius: 15px;
 width: calc(100%-10px);
 background: white;
 margin-right: 5px;
 color: {CustomColors.DarkMainString};
}}

.answerString{{
 font-family: Nunito;
 font-size: 12pt;
 color: {CustomColors.DarkMainString};
}}

.feedback{{
 font-family: Nunito;
 font-size: 12pt;
 color: {CustomColors.DarkMainString};
}}

.feedback > p{{
 font-family: Nunito;
 font-size: 12pt;
 color: {CustomColors.DarkMainString} !important;
}}

p.buttonLine{{
margin-top: 0px;
margin-bottom: 5px;
}}

p.wideLine{{
    line-height: 2;
}}

a{{
    color:white;
}}
html{{
    background: {CustomColors.LightMainString};

    -webkit-touch-callout: none; /* iOS Safari */
    -webkit-user-select: none; /* Safari */
     -khtml-user-select: none; /* Konqueror HTML */
       -moz-user-select: none; /* Firefox */
        -ms-user-select: none; /* Internet Explorer/Edge */
            user-select: none; /* Non-prefixed version, currently
                                  supported by Chrome and Opera */
}}

.blueText{{
color: {CustomColors.DarkMainString};
float:left;
margin:15;
margin-left: -7;
}}

textarea{{
resize:none;
margin-bottom: 20;
width: 100%;
height:35%;
font-size   : 13pt;
border-color:{CustomColors.DarkMainString};
color: {CustomColors.DarkMainString};
}}

.textarea{{
margin: 0;
width: 100%;
padding-left: 7px;
padding-right: 7px;
box-sizing: border-box;
-moz-box-sizing: border-box;
-webkit-box-sizing: border-box;
}}

.answerButton{{
width:40;
height:40;
float:right;
margin-right: -2px;
margin-top: 0px;
background: transparent;
border: 0px;
clear:both;
}}

.answerButton>img{{
width:40;
height:40;
margin-bottom:0;
}}

.trueButton{{
  text-align: center;
  margin:0;
  padding     : 0;
  height:40;
  width:120;  
  background: {CustomColors.DarkMainString};
  border-radius: 0px 14px 0px 14px; 
-moz-border-radius: 14px 0px 0px 14px; 
-webkit-border-radius: 14px 0px 0px 14px; 
border: 0px solid transparent;
}}

.falseButton{{
  text-align: center;
  margin:0;
  padding     : 0;
  height:40;
  width:120;  
  background: white;
  border-radius: 0px 14px 14px 0px; 
-moz-border-radius: 0px 14px 14px 0px; 
-webkit-border-radius: 0px 14px 14px 0px;
border: 2px solid {CustomColors.DarkMainString};
}}

.trueButton > span{{
text-align: center;
  margin:auto;  
  color: white;
width: 120;
font-family: Nunito;
font-size: 12pt;
}}

.falseButton > span{{
text-align: center;
width: 120;
  margin:0;  
  color: {CustomColors.DarkMainString};
  background: white;
font-family: Nunito;
font-size: 12pt;
}}

table {{
    border-collapse: collapse;
	color:white;
	font-size: 11pt;
    width: calc(100% - 14px);
    margin-bottom: 20;
    font-family: 'Roboto-Light';
    margin-right: 7px;
    margin-left: 7px;
}}

table, th, td {{
    border: 2px solid white;
	padding:5;
}}

td{{
	padding:5px 0;
}}

.selectBox select {{
    border-radius: 0;
    background: transparent;
    border: 0;
    font-size: 12pt;
	color:white;  
	width: 100%;
	height: 100%;
   }}
   
select::-moz-focus-inner {{
  border: 5 0 5 0;
}}

select:focus {{outline:none;}} 

select>option{{
color: {CustomColors.DarkMainString};
}}
   
optgroup {{ display: none; }}

.selectBox {{
-webkit-border-radius: 5px;
   -moz-border-radius: 5px;
   display: block;
   border-radius: 5px;
   height: 25px;
    border: 1px solid white; 
   background-color: {CustomColors.DarkMainString};
   font-size: 12pt;
   color: white;
    margin: 5 5 5 5;
}}

.selectBox select.wide{{
	white-space: normal;
}}

.selectBox.wide{{
   height: 50px;
}}

td>p>span.selectBox select{{
	margin-left: 5px;
	margin-right: 5px;
	width: calc(100% - 10px);
}}

td>p>span.selectBox{{
	width: 100%;
	display: inline-block;
	margin-left: -7px;
	margin-right: -7px;
}}

p>span.selectBox select{{
    width: 135px;
}}

p>span.selectBox{{
    width: 140px;
   display: inline-block;
}}

img{{
    max-width: 100%;
margin-bottom: 20;
}}

.answerWord{{
font-weight: bold;
color: {CustomColors.DarkMainString};
}}

table.invisible{{
width: auto;
max-width: 100%;
}}

.invisible {{
border: 0px none transparent;
padding:1;
}}



center{{
display: flex;
flex-direction: row;
justify-content: center;
}}

.leftColumnHeader{{
  text-align: center;
  margin:0;
  padding     : 0;
  height:50;
  width:160;  
  background: {CustomColors.DarkMainString};
  border-radius: 0px 14px 0px 14px; 
-moz-border-radius: 14px 0px 0px 14px; 
-webkit-border-radius: 14px 0px 0px 14px; 
border: 0px solid transparent;
align-self: center;
justify-self: center;
}}

.rightColumnHeader{{
  text-align: center;
  margin:0;
  padding     : 0;
  height:50;
  width:160;  
  background: white;
  border-radius: 0px 14px 14px 0px; 
-moz-border-radius: 0px 14px 14px 0px; 
-webkit-border-radius: 0px 14px 14px 0px;
border: 2px solid #004876;
justify-self: center;
align-self: center; 
}}

.leftColumnHeader > span{{
text-align: center;
  margin:auto;  
  color: white;
width: 150;
font-family: Nunito;
font-size: 12pt;
}}

.rightColumnHeader > span{{
text-align: center;
width: 150;
  margin:0;  
  color: {CustomColors.DarkMainString};
  background: white;
font-family: Nunito;
font-size: 12pt;
}}

.container {{
  display: -webkit-box;
  display: -ms-flexbox;
  display: flex;
  width: 100%;
}}

.columns {{
  display: -webkit-box;
  display: -ms-flexbox;
  display: flex;
  -webkit-box-align: start;
  -ms-flex-align: start;
  align-items: flex-start;
  margin: 0 auto;
}}

.column {{
  width: 110px;
  margin: 0 auto;
  background: transparent;
}}

.orderColumn{{
  width: 280px;
  margin: 0 auto;
  background: transparent;
}}

.column-header {{
  padding: 0.1rem;
  background: {CustomColors.DarkMainString};
  color: white;
}}

.column-empty-header{{
  padding: 0.1rem;
  background: transparent;
  color: transparent;
}}

.column-empty-header p{{
  color: transparent;
}}

.column-header p {{
  text-align: center;
}}

.task-list {{
  min-height: 3rem;    
  padding-bottom: 3rem;
}}

ul {{
  list-style-type: none;
  margin: 0;
  padding: 0;
}}

columns > li {{
  list-style-type: none;
}}

task-list > li {{
  list-style-type: none;
}}

.ql-indent-1{{
    margin-left: 16px;
}}

.task {{
  display: -webkit-box;
  display: -ms-flexbox;
  display: flex;
  -webkit-box-pack: center;
  -ms-flex-pack: center;
  justify-content: center;
  vertical-align: middle;
  list-style-type: none;
  background: Gray;
  border: solid 2px Gray;
  border-radius:1.9rem;
  margin: 5px;
  padding-top: 10px;
  padding-bottom: 10px; 
  text-align: center;
  vertical-align: middle;
}}

.task p {{
  margin: auto;
  color: white;
  text-align: center;
}}

.leftColumn >.task {{
  display: -webkit-box;
  display: -ms-flexbox;
  display: flex;
  -webkit-box-pack: center;
  -ms-flex-pack: center;
  justify-content: center;
  vertical-align: middle;
  list-style-type: none;
  background: {CustomColors.DarkMainString};
  border: solid 2px {CustomColors.DarkMainString};
  border-radius:1.9rem;
  margin: 5px;
  padding-top: 10px;
  padding-bottom: 10px; 
  text-align: center;
  vertical-align: middle;
}}

.leftColumn > .task p {{
  margin: auto;
  color: white;
}}

.rightColumn >.task {{
  display: -webkit-box;
  display: -ms-flexbox;
  display: flex;
  -webkit-box-pack: center;
  -ms-flex-pack: center;
  justify-content: center;
  vertical-align: middle;
  list-style-type: none;
  background: white;
  border: solid 2px {CustomColors.DarkMainString};
  border-radius:1.9rem;
  margin: 5px;
  padding-top: 10px;
  padding-bottom: 10px; 
  text-align: center;
  vertical-align: middle;
}}

.rightColumn > .task p {{
  margin: auto;
  color: {CustomColors.DarkMainString};
}}

.orderColumn >.task {{
  display: flex;
  justify-content: center;
  vertical-align: middle;
  list-style-type: none;
  background: {CustomColors.DarkMainString};
  border: solid 2px {CustomColors.DarkMainString};
  border-radius:1.9rem;
  margin: 5px;
  padding-top: 10px;
  padding-bottom: 10px; 
  text-align: center;
  vertical-align: middle;
}}

.orderColumn > .task p {{
  margin: auto;
  color: white;
  text-align: center;
}}

#taskText {{
  background: #fff;
  border: #000013 0.15rem solid;
  border-radius: 0.2rem;
  text-align: center;
  height: 4rem;
  width: 7rem;
  margin: auto 0.8rem auto 0.1rem;
}}


.katex {{ font-family: 'Roboto-Light' !important; font-size: 12pt !important; }}
</style>

<script>
window.onload=function(){{
	var selects = document.getElementsByTagName('select');
		
	for (i=0;i<selects.length;i++){{
			selects[i].onfocus = function(){{
				this.parentNode.style.borderColor = '#e30918';
			}}
			selects[i].onblur = function(){{
				this.parentNode.style.borderColor = 'white';
			}}
	}}
}}
</script>
<script src=""Sortable.js""></script>
<script src=""auto-render.min.js""></script>
<script src=""katex.min.js""></script>
<link rel=""stylesheet"" href=""katex.min.css"">
";

        public static string OlQuestionsScripts = $@"
<script>
window.onload = function(){{ 
var a = new Sortable(document.getElementById('order'), {{group: 'shared',animation: 150}});
document.getElementById('order').on('touchstart',function(event) {{event.preventDefault(); }}, false);
window.addEventListener('touchmove', function () {{ }});
document.getElementsByClassName('answerButton')[0].setAttribute('onclick',
    function(){{
        var elements = document.getElementById(""order"").getElementsByClassName(""task"");
	    var result = """";
	    for (var i = 0; i < elements.length; i++) {{
	      result = result + ""responce"" + (i+1) + ""="" + elements[i].getElementsByTagName(""p"")[0].innerText;
	      if (i != elements.length - 1)
		    result = result + ""&"";
	    }}
        window.location.href = location.protocol + '//' + location.host + location.pathname + ""?"" + result;
    }});
}};


function getOrderedResult(){{
    var elements = document.getElementById(""order"").getElementsByClassName(""task"");
    var result = """";
    for (var i = 0; i < elements.length; i++) {{
      result = result + ""responce"" + (i+1) + ""="" + elements[i].getElementsByTagName(""p"")[0].innerText;
      if (i != elements.length - 1)
	    result = result + ""&"";
    }}
    window.location.href = location.protocol + '//' + location.host + location.pathname + ""?"" + result;
}}
</script>
";

        public static string SwQuestionsScripts = $@"
<script type=""text/javascript"">
window.onload= function(){{

var a = new Sortable(document.getElementById('center'), {{group: ""name""}});
var b = new Sortable(document.getElementById('responce1'), {{group: ""name""}});
var c = new Sortable(document.getElementById('responce2'), {{group: ""name""}});
window.addEventListener('touchmove', function () {{ }});

document.getElementsByClassName(""answerButton"")[0].addEventListener(""click"",
    function() {{
        var result = getResultForResponce(""responce1"");
	    
	    result = result + ""&"";
	    result = result + getResultForResponce(""responce2"");
	    
        window.location.href = location.protocol + '//' + location.host + location.pathname + ""?"" + result;
    }});
}}

function getResultForResponce(responce){{
    var elements = document.getElementById(responce).getElementsByClassName(""task"");
	var result = """";
	for (var i = 0; i < elements.length; i++) {{
        // Symbol = is forbidden inside the answers
        var ans = elements[i].getElementsByTagName(""p"")[0].innerText.replace(""="", ""equal"").replace(""/"", ""div"");
	  result = result + ans  + ""="" + responce;
	  if (i != elements.length - 1)
		result = result + ""&"";
	}}
	return result;
}}
</script>
";
    }
}
