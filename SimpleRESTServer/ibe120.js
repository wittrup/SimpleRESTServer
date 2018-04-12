        function visTabell(arrayAvObjekt, hasDelCol = false) {
            //lager html-tabell av arrayet

            var tabellEl = document.createElement("table");
            var tbodyEl = document.createElement("tbody");
            var theadEl = document.createElement("thead");

            //lager overskriftsraden
            var tittelrad = "<tr>";
            var hasRows = false;
            for (var egenskap in arrayAvObjekt[0]) { //bruker egenskapene til objektene som overskrift i arrayet
                tittelrad += "<th>" + egenskap + "</th>";
                hasRows = true;
            }
            if (hasRows && hasDelCol) {
                tittelrad += "<th>Slett</th>";
            }
            tittelrad += "</tr>";
            theadEl.innerHTML = tittelrad; //legger overskriftsraden inn i thead
            tabellEl.appendChild(theadEl); //legger thead inn i tabellen

            //lager tbody
            for (var y = 0; y < arrayAvObjekt.length; y++) {
                var rad = "<tr>"; //starter ny rad i tabellen
                for (var egenskap in arrayAvObjekt[y]) {
                    rad += "<td>" + arrayAvObjekt[y][egenskap] + "</td>"; //legger inn data i en celle i tabellen
                }
                if (hasDelCol) {
                    rad += "<td><input type=\"button\" value=\"X\" onclick=\"slettAktivitet(" + y + ")\"/></td>";
                }
                rad += "</tr>";
                tbodyEl.innerHTML += rad; //legger den ferdige tabellraden inn i tabellens body
            }
            tabellEl.appendChild(tbodyEl); //legger tabellbody inn i tabellelementet
            return tabellEl;
        }
