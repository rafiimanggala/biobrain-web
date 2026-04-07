export class ContentStyles {

  static getAnswerStyles({ primary }: {primary: string }) {
    return `
    <style>
    <link href="https://fonts.googleapis.com/css2?family=Nunito:ital,wght@0,600;0,700;0,800;0,900;1,600;1,700;1,800;1,900&family=Roboto:ital,wght@0,100;0,300;0,400;0,500;1,100;1,300;1,400;1,500&display=swap" rel="stylesheet">
    html{
        color: ${primary};
        font-family: 'Nunito', sans-serif;
        font-size: 12pt;

        -webkit-touch-callout: none; /* iOS Safari */
        -webkit-user-select: none; /* Safari */
         -khtml-user-select: none; /* Konqueror HTML */
           -moz-user-select: none; /* Firefox */
            -ms-user-select: none; /* Internet Explorer/Edge */
                user-select: none; /* Non-prefixed version, currently
                                      supported by Chrome and Opera */
    }
    
    div{
        -webkit-touch-callout: none; /* iOS Safari */
        -webkit-user-select: none; /* Safari */
        -khtml-user-select: none; /* Konqueror HTML */
            -moz-user-select: none; /* Old versions of Firefox */
            -ms-user-select: none; /* Internet Explorer/Edge */
                user-select: none; /* Non-prefixed version, currently
                                    supported by Chrome, Edge, Opera and Firefox */
    }
    p,ul,ol,li,div{
        color: ${primary};
    }
    a{
        color: ${primary};
    }
    img{
        max-width: 100%;
        pointer-events: none;
        height: auto !important; 
    }
    .correctString{
        font-size: 16pt !important;
    }

    table {
        border-collapse: collapse;
        color: ${primary};
        font-size: 12pt;
        width: 95%;
        max-width:100%;
        margin-bottom: 10;
    }

    table, th, td {
        border: 2px solid ${primary};
        padding:5;
    }
    .ql-indent-1{
        margin-left: 16px;
    }
    .katex { font-family: 'Roboto', sans-serif !important; font-size: 12pt !important; }
    </style>
    <link href="https://fonts.googleapis.com/css2?family=Nunito:ital,wght@0,600;0,700;0,800;0,900;1,600;1,700;1,800;1,900&family=Roboto:ital,wght@0,100;0,300;0,400;0,500;1,100;1,300;1,400;1,500&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/katex@0.13.11/dist/katex.min.css" integrity="sha384-Um5gpz1odJg5Z4HAmzPtgZKdTBHZdw8S29IecapCSB31ligYPhHQZMIlWLYQGVoc" crossorigin="anonymous">
    <script defer src="https://cdn.jsdelivr.net/npm/katex@0.13.11/dist/katex.min.js" integrity="sha384-YNHdsYkH6gMx9y3mRkmcJ2mFUjTd0qNQQvY9VYZgQd7DcN7env35GzlmFaZ23JGp" crossorigin="anonymous"></script>
    <script defer src="https://cdn.jsdelivr.net/npm/katex@0.13.11/dist/contrib/auto-render.min.js" integrity="sha384-vZTG03m+2yp6N6BNi5iM4rW4oIwk5DfcNdFfxkk9ZWpDriOkXX8voJBFrAO7MpVl" crossorigin="anonymous"
        onload="renderMathInElement(document.body);"></script>
    `;
  }

  static getMaterialStyles({ primary, accent }: {primary: string; accent: string}): string {
    return `
<style>
html{
    -webkit-text-size-adjust: 100%;

    -webkit-touch-callout: none; /* iOS Safari */
    -webkit-user-select: none; /* Safari */
     -khtml-user-select: none; /* Konqueror HTML */
       -moz-user-select: none; /* Firefox */
        -ms-user-select: none; /* Internet Explorer/Edge */
            user-select: none; /* Non-prefixed version, currently
                                  supported by Chrome and Opera */
    padding: 8px;
}

b{
    font-family: 'Roboto', sans-serif;
    font-weight: 600;
}

strong{
    font-family: 'Roboto', sans-serif;
    font-weight: 600;
}

details summary::-webkit-details-marker {
    display:none;
  }

summary:before {
    content: "navigate_next";
    font-family: "Material Icons", sans-serif !important;
    font-style: normal;    
    font-stretch: 100%;
    font-weight: 400 !important;
    font-size: 24px;
    color: white;
    font-weight: bold;
    float: left;
}

details[open] summary:before {
    transform: rotate(90deg);
}

summary::selection{
    background: transparent;
}

summary{
    color: white;
    background-color: ${primary};
    font-family: 'Nunito', sans-serif;
    font-size: 14px;
    margin-left: -24px;
    margin-bottom: 4px;
    list-style-type: none;
    font-weight: bold;
    cursor: pointer;
    padding: 16px 8px;
    padding-right: 55px;
    border-radius: 2px;
    clear: both;
    position: relative;
}

summary>span{    
    vertical-align: bottom;
    white-space: normal; 
    display: inline-block;
    position: sticky;
    left: 30px;  
    width: calc(100% - 71px);
    padding: 0px;
}

.header-actions>button{
    border: none;
    background: none;
    cursor: pointer;
    min-width: 25.5px;
    display: inline-block;
}

.header-actions>img{    
    margin-left: 10px;
    display: inline-block;
}

.header-actions{
    display: inline-block;
    margin-right: auto;
    position: absolute;
    right: 16px;
  }

.right-margin{
    margin-right: 10px;
}

.devider{
    margin-left: auto;
    display: none;
}

.heading-icon{
    height: 18px !important;
    margin-top: 0px !important;
    margin-bottom: 0px !important;
}

.embed-video{
    margin-left: auto;
    margin-right: auto;
    width: 100%;
    height: 295px;
    max-width: 450px;
    margin-bottom: 10px;
}

p{
    -webkit-touch-callout: none; /* iOS Safari */
    -webkit-user-select: none; /* Safari */
    -khtml-user-select: none; /* Konqueror HTML */
        -moz-user-select: none; /* Old versions of Firefox */
        -ms-user-select: none; /* Internet Explorer/Edge */
            user-select: none; /* Non-prefixed version, currently
                                supported by Chrome, Edge, Opera and Firefox */
}
    
div{
    -webkit-touch-callout: none; /* iOS Safari */
    -webkit-user-select: none; /* Safari */
    -khtml-user-select: none; /* Konqueror HTML */
        -moz-user-select: none; /* Old versions of Firefox */
        -ms-user-select: none; /* Internet Explorer/Edge */
            user-select: none; /* Non-prefixed version, currently
                                supported by Chrome, Edge, Opera and Firefox */
}

details{
-webkit-touch-callout: none; /* iOS Safari */
    -webkit-user-select: none; /* Safari */
    -khtml-user-select: none; /* Konqueror HTML */
        -moz-user-select: none; /* Old versions of Firefox */
        -ms-user-select: none; /* Internet Explorer/Edge */
            user-select: none; /* Non-prefixed version, currently
                                supported by Chrome, Edge, Opera and Firefox */
    color: #212121;
    font-size: 14px;
    font-family: 'Roboto', sans-serif;
    margin-right: 7px;
    margin-left: 0px;
    max-width: 100%;
    padding-left: 24px;
    padding-right: 5px;
}

summary:focus {
\toutline-style: none;
}

details > p {
    margin-top: 0px;
    margin-bottom: 20px;
}

ol {
    Margin-top: 0pt;
    margin-bottom: 10px;
}

ul {
    Margin-top: 0pt;
    margin-bottom: 10px;
}

li{
    margin-bottom: 5px;
}

a{
    color:#696969;
    text-decoration: underline;
    cursor: pointer;
}

table {
    border-collapse: collapse;
    font-size: 14px;
    color:#696969;
    margin: auto;
    max-width: 100%;
    margin-bottom: 20px;
}

table, th, td {
    border: 2px solid ${primary};
    padding: 10px;
    word-wrap: break-word;
}

th{
    font-family: 'Roboto', sans-serif;
    font-weight: bold;
    padding: 10px;
}

td{
    vertical-align: top;
    padding: 10px;
}

td > ul{
    padding-left: 18px;
}

td > ol{
    padding-left: 18px;
}

td.diagonalCross
{
\tposition:   relative;
\tbackground: linear-gradient(to bottom right,  transparent calc(50% - 1px), red, transparent calc(50% + 1px));
}

td.diagonalCross:before
{
\tcontent:     '"';
\tdisplay:     block;
\tposition:    absolute;
\twidth:       100%;
\theight:      100%;
\ttop:         0;
\tleft:        0;
\tz-index:     -1;
\tbackground:  linear-gradient(to top right, transparent calc(50% - 1px), red, transparent calc(50% + 1px));
}

details img{
    display: block;
    max-width: 100%;
    height: auto !important; 
    margin-bottom: 15px;
    margin-top: 10px;
    pointer-events: none;
}

img{
    max-width: 100%;
    height: auto !important; 
}

.heading-icon{
    pointer-events: auto;
}

details.excluded > summary{
    background-color: #9e9e9e !important;
    opacity: 0.6;
}

details.excluded > summary > span{
    text-decoration: line-through;
}

details.excluded > *:not(summary){
    opacity: 0.55;
}

.exclude-button{
    background-color: white !important;
    color: ${primary} !important;
    border: none;
    border-radius: 4px;
    cursor: pointer;
    font-family: 'Nunito', sans-serif;
    font-size: 12px;
    font-weight: bold;
    padding: 4px 10px;
    margin-left: 8px;
    pointer-events: auto;
    text-transform: uppercase;
}

details.excluded > summary .exclude-button{
    background-color: #ffeb3b !important;
    color: #212121 !important;
}
</style>
<link href="https://fonts.googleapis.com/css2?family=Nunito:ital,wght@0,600;0,700;0,800;0,900;1,600;1,700;1,800;1,900&family=Roboto:ital,wght@0,100;0,300;0,400;0,500;1,100;1,300;1,400;1,500&display=swap" rel="stylesheet">
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/katex@0.13.11/dist/katex.min.css" integrity="sha384-Um5gpz1odJg5Z4HAmzPtgZKdTBHZdw8S29IecapCSB31ligYPhHQZMIlWLYQGVoc" crossorigin="anonymous">
<script defer src="https://cdn.jsdelivr.net/npm/katex@0.13.11/dist/katex.min.js" integrity="sha384-YNHdsYkH6gMx9y3mRkmcJ2mFUjTd0qNQQvY9VYZgQd7DcN7env35GzlmFaZ23JGp" crossorigin="anonymous"></script>
<script defer src="https://cdn.jsdelivr.net/npm/katex@0.13.11/dist/contrib/auto-render.min.js" integrity="sha384-vZTG03m+2yp6N6BNi5iM4rW4oIwk5DfcNdFfxkk9ZWpDriOkXX8voJBFrAO7MpVl" crossorigin="anonymous"
    onload="renderMathInElement(document.body);"></script>
    `;
  }

  static getQuestionStyles({ primary, accent }: {primary: string; accent: string}): string {
    return `<style>

    button{
        clear:both;
        width: 100%;
        margin-right: 7px;
        padding     : 0;
        border      : 0;
        background: ${primary};
        height: auto;
        display: inline-table;
        font-size: 12pt !important;
        cursor: pointer;
    }

    .letter{
        padding:0;
        text-align: center;
        background: white;
        color: ${primary};
        width:40px;
        height:40px;
        display: table-cell;
        vertical-align:middle;
        font-family: 'Nunito', sans-serif;
        font-size: 12pt;
        font-weight: bold;
    }

    button > span{
        text-align: left;
        background-color: ${primary};
        white-space: normal;
         vertical-align:middle;
        margin:10px;
        padding-left: 10px;
        padding-right: 10px;
        font-family: 'Nunito', sans-serif;
        font-size: 12pt !important;
        color: white;
        display: table-cell;
    }

    .leftPair{
        display: table-cell;
        width:100%;
    }

    .rightPair{
        display: table-cell;
        width:100%;
    }

    .leftPair>.buttonLine{
        margin-right: 2.5px;
    }

    .rightPair>.buttonLine{
        margin-left: 2.5px;
    }

    .container{
        display: inline-table;
        width:100%;
    }

    .test{
        background: ${accent};
        height:100%;
    }

    .questionText{
        color:white;
        font-family: 'Nunito', sans-serif;
        font-size: 13pt;
        margin-bottom:25px;
        margin-top: 15px;
        margin-right: 7px;
        margin-left: 7px;
    }

    .questionNumber {
        float: left;
        margin-bottom:15px;
    }

    .questionBody{
        clear: both;
        margin-top: 15px;
    }

    .levelName {
        text-align: center;
        position: relative;
        right: 25%;
        width: 50%;
        float: right;
        margin-bottom:15px;
    }

    p{
        color: white;
        font-family: 'Roboto', sans-serif;
        font-size: 12pt;
        text-align: left;
        margin-bottom: 20px;
        margin-right: 7px;
        margin-left: 7px;
    }

    ul,ol{        
        color: white;
        font-family: 'Roboto', sans-serif;
        font-size: 12pt;
    }

    .correctString{
        font-family: 'Nunito', sans-serif;
        font-size: 16pt;
        color: ${primary};
    }

    .correctBlock{
        border: 4px solid ${primary};
        border-radius: 15px;
        width: calc(100%-10px);
        background: white;
        margin-right: 5px;
        color: ${primary};
    }

    .answerString{
        font-family: 'Nunito', sans-serif;
        font-size: 12pt;
        color: ${primary};
    }

    .feedback{
        font-family: 'Nunito', sans-serif;
        font-size: 12pt;
        color: ${primary};
    }

    .feedback > p{
        font-family: 'Nunito', sans-serif;
        font-size: 12pt;
        color: ${primary} !important;
    }

    p.buttonLine{
        margin-top: 0px;
        margin-bottom: 5px;
    }

    p.wideLine{
    }

    a{
        color:white;
    }
    
    p{
        -webkit-touch-callout: none; /* iOS Safari */
        -webkit-user-select: none; /* Safari */
        -khtml-user-select: none; /* Konqueror HTML */
            -moz-user-select: none; /* Old versions of Firefox */
            -ms-user-select: none; /* Internet Explorer/Edge */
                user-select: none; /* Non-prefixed version, currently
                                    supported by Chrome, Edge, Opera and Firefox */
    }
    
    div{
        -webkit-touch-callout: none; /* iOS Safari */
        -webkit-user-select: none; /* Safari */
        -khtml-user-select: none; /* Konqueror HTML */
            -moz-user-select: none; /* Old versions of Firefox */
            -ms-user-select: none; /* Internet Explorer/Edge */
                user-select: none; /* Non-prefixed version, currently
                                    supported by Chrome, Edge, Opera and Firefox */
    }

    html{

        -webkit-touch-callout: none; /* iOS Safari */
        -webkit-user-select: none; /* Safari */
         -khtml-user-select: none; /* Konqueror HTML */
           -moz-user-select: none; /* Firefox */
            -ms-user-select: none; /* Internet Explorer/Edge */
                user-select: none; /* Non-prefixed version, currently
                                      supported by Chrome and Opera */
    }

    .blueText{
        color: ${primary};
        float:left;
        margin:15px;
        margin-left: -7px;
    }

    textarea{
        font-family: 'Nunito', sans-serif;
        resize:none;
        margin-bottom: 20px;
        width: 100%;
        height:100px;
        font-size   : 13pt;
        border-color: ${primary};
        color: ${primary};
        outline: none;
    }

    textarea{
        margin: 0;
        width: 100%;
        padding: 7px;
        box-sizing: border-box;
        -moz-box-sizing: border-box;
        -webkit-box-sizing: border-box;
    }

    .answerButton{
        width:40px;
        height:40px;
        float:right;
        margin-right: -2px;
        margin-top: 0px;
        background: transparent;
        border: 0px;
        clear:both;
    }

    .answerButton>img{
        width:40px;
        height:40px;
        margin-bottom:0px;
    }

    center{
        display: flex;
        justify-content: center;
        align-items: center;
    }

    .trueButton{
        text-align: center;
        margin:0;
        padding: 0;
        height:40px;
        width:120px;
        background: ${primary};
        border-radius: 0px 14px 0px 14px;
        -moz-border-radius: 14px 0px 0px 14px;
        -webkit-border-radius: 14px 0px 0px 14px;
        border: 0px solid transparent;
    }

    .falseButton{
        text-align: center;
        margin:0;
        padding:0;
        height:40px;
        width:120px;
        background: white;
        border-radius: 0px 14px 14px 0px;
        -moz-border-radius: 0px 14px 14px 0px;
        -webkit-border-radius: 0px 14px 14px 0px;
        border: 2px solid ${primary};
    }

    .trueButton > span{
        text-align: center;
        margin:auto;
        color: white;
        width: 120px;
        font-family: 'Nunito', sans-serif;
        font-size: 12pt;
        background: transparent;
    }

    .falseButton > span{
        text-align: center;
        width: 120px;
        margin:0;
        color: ${primary};
        background: transparent;
        font-family: 'Nunito', sans-serif;
        font-size: 12pt;
    }

    table {
        border-collapse: collapse;
        color:white;
        font-size: 11pt;
        width: calc(100% - 14px);
        margin-bottom: 20px;
        font-family: 'Roboto', sans-serif;
        margin-right: 7px;
        margin-left: 7px;
    }

    table, th, td {
        border: 2px solid white;
        padding:5px;
    }

    .selectBox select {
        border-radius: 0;
        background: transparent;
        border: 0;
        font-size: 12pt;
        color:white;
        width: 100%;
        height: 100%;
    }

    select::-moz-focus-inner {
        border: 5px 0px 5px 0px;
    }

    select:focus {outline:none;}

    select>option{
        color: ${primary};
    }

    optgroup { display: none; }

    .selectBox {
        -webkit-border-radius: 5px;
        -moz-border-radius: 5px;
        display: inline-block;
        border-radius: 5px;
        height: 25px;
        border: 1px solid white;
        background-color: ${primary};
        font-size: 12pt;
        color: white;
        margin: 5px 5px 5px 5px;
    }

    .selectBox select.wide{
        white-space: normal;
    }

    .selectBox.wide{
       height: 50px;
    }

    p>span.selectBox select{
        width: 135px;
    }

    p>span.selectBox{
        width: 140px;
       display: inline-block;
    }

    img{
        max-width: 100%;
        height: auto !important; 
        margin-bottom: 20px;
        pointer-events: none;
    }

    .answerWord{
        font-weight: bold;
        color: ${primary};
    }

    table.invisible{
        width: auto;
        max-width: 100%;
    }

    .invisible {
        border: 0px none transparent;
        padding:1;
    }

    .columnHeaders{
        display: flex;
        flex-direction: row;
        justify-content: center;
        grid-auto-rows: 1fr;
    }

    .leftColumnHeader{
        display: flex;
        flex-direction: row;
        justify-content: center;
        text-align: center !important;
        margin:0 !important;
        padding: 0 !important;
        padding-left: 14px !important;
        width:160px !important;
        background: ${primary} !important;
        border-radius: 0px 14px 0px 14px !important;
        -moz-border-radius: 14px 0px 0px 14px !important;
        -webkit-border-radius: 14px 0px 0px 14px !important;
        border: 0px solid transparent !important;
    }

    .rightColumnHeader{
        display: flex;
        flex-direction: row;
        justify-content: center;
        text-align: center !important;
        margin:0 !important;
        padding: 0 !important;
        padding-right: 14px !important;
        width:160px !important;
        background: white !important;
        border-radius: 0px 14px 14px 0px !important;
        -moz-border-radius: 0px 14px 14px 0px !important;
        -webkit-border-radius: 0px 14px 14px 0px !important;
        border: 2px solid ${primary} !important;
    }

    .leftColumnHeader > span{
        align-self: center;
        text-align: center !important;
        margin:auto !important;
        color: white !important;
        width: 150px !important;
        font-family: 'Nunito', sans-serif !important;
        font-size: 12pt !important;
    }

    .rightColumnHeader > span{
        align-self: center;
        text-align: center !important;
        width: 150px !important;
        margin:0 !important;
        color: ${primary} !important;
        background: white !important;
        font-family: 'Nunito', sans-serif !important;
        font-size: 12pt !important;
    }

    .container {
        display: -webkit-box;
        display: -ms-flexbox;
        display: flex;
        width: 100%;
    }

    .columns {
        display: -webkit-box;
        display: -ms-flexbox;
        display: flex;
        -webkit-box-align: start;
        -ms-flex-align: start;
        align-items: flex-start;
        margin: 0 auto;
        list-style-type: none;
        padding: 0;
    }    

    columns li {
        list-style-type: none;
    }

    .column {
        width: 110px;
        margin: 0 auto;
        background: transparent;
    }

    .orderColumn{
        width: 220px;
        margin: 0 auto;
        background: transparent;
    }

    .column-header {
        padding: 0.1rem;
        background: ${primary};
        color: white;
    }

    .column-empty-header{
        padding: 0.1rem;
        background: transparent;
        color: transparent;
    }

    .column-empty-header p{
        color: transparent;
    }

    .column-header p {
        text-align: center;
    }

    .task-list {
        min-height: 3rem;
        padding: 0;
        padding-bottom: 3rem;
        list-style-type: none;
        margin: 0;
    }

    task-list li {
        list-style-type: none;
    }

    .ql-indent-1{
        margin-left: 16px;
    }

    .task {
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
        cursor: pointer;
    }

    .task p {
        margin: auto;
        color: white;
        text-align: center;
    }

    .leftColumn >.task {
        display: -webkit-box;
        display: -ms-flexbox;
        display: flex;
        -webkit-box-pack: center;
        -ms-flex-pack: center;
        justify-content: center;
        vertical-align: middle;
        list-style-type: none;
        background: ${primary};
        border: solid 2px ${primary};
        border-radius:1.9rem;
        margin: 5px;
        padding-top: 10px;
        padding-bottom: 10px; 
        text-align: center;
        vertical-align: middle;
    }

    .leftColumn > .task p {
        margin: auto;
        color: white;
    }

    .rightColumn >.task {
        display: -webkit-box;
        display: -ms-flexbox;
        display: flex;
        -webkit-box-pack: center;
        -ms-flex-pack: center;
        justify-content: center;
        vertical-align: middle;
        list-style-type: none;
        background: white;
        border: solid 2px ${primary};
        border-radius:1.9rem;
        margin: 5px;
        padding-top: 10px;
        padding-bottom: 10px; 
        text-align: center;
        vertical-align: middle;
    }

    .rightColumn > .task p {
        margin: auto;
        color: ${primary};
    }

    .orderColumn >.task {
        display: flex;
        justify-content: center;
        vertical-align: middle;
        list-style-type: none;
        background: ${primary};
        border: solid 2px ${primary};
        border-radius:1.9rem;
        margin: 0.4rem;
        text-align: center;
        vertical-align: middle;
    }

    .orderColumn > .task p {
        margin: auto;
        color: white;
        text-align: center;
    }

    #taskText {
        background: #fff;
        border: #000013 0.15rem solid;
        border-radius: 0.2rem;
        text-align: center;
        height: 4rem;
         width: 7rem;
        margin: auto 0.8rem auto 0.1rem;
    }


    .katex { font-family: 'Roboto', sans-serif !important; font-size: 12pt !important; }
    </style>
    <link href="https://fonts.googleapis.com/css2?family=Nunito:ital,wght@0,600;0,700;0,800;0,900;1,600;1,700;1,800;1,900&family=Roboto:ital,wght@0,100;0,300;0,400;0,500;1,100;1,300;1,400;1,500&display=swap" rel="stylesheet">
    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/katex@0.13.11/dist/katex.min.css" integrity="sha384-Um5gpz1odJg5Z4HAmzPtgZKdTBHZdw8S29IecapCSB31ligYPhHQZMIlWLYQGVoc" crossorigin="anonymous">
    <script defer src="https://cdn.jsdelivr.net/npm/katex@0.13.11/dist/katex.min.js" integrity="sha384-YNHdsYkH6gMx9y3mRkmcJ2mFUjTd0qNQQvY9VYZgQd7DcN7env35GzlmFaZ23JGp" crossorigin="anonymous"></script>
    <script defer src="https://cdn.jsdelivr.net/npm/katex@0.13.11/dist/contrib/auto-render.min.js" integrity="sha384-vZTG03m+2yp6N6BNi5iM4rW4oIwk5DfcNdFfxkk9ZWpDriOkXX8voJBFrAO7MpVl" crossorigin="anonymous"
        onload="renderMathInElement(document.body);"></script>



    `;
  }
}
