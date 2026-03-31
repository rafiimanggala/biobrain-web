import { Injectable } from "@angular/core";
import { GetCurriculaWithCountryRelationQuery_Country_Result, GetCurriculaWithCountryRelationQuery_Result } from "src/app/api/curricula/get-curricula-with-country-relation.query";

@Injectable({
    providedIn: 'root',
})
export class CurriculumService {

    filterCurricula(country: string, state: string, curricula: GetCurriculaWithCountryRelationQuery_Result[]): GetCurriculaWithCountryRelationQuery_Result[] {
        let filteredCurricula: GetCurriculaWithCountryRelationQuery_Result[] = [];

        curricula.forEach(c => {
            if (!c.availableCountries) {
                filteredCurricula.push(c);
                return;
            }
            var matchedCountries = c.availableCountries.filter(x => x.name.toLowerCase() == country);

            // If curriculum excluded from contry and selected contry is not in list we should add this curriculum
            if (c.availableCountries.some(_ => _.isExclude)) {
                if (!matchedCountries || matchedCountries.length < 1)
                    filteredCurricula.push(c);
            }
            // If curriculum not excluded
            else {
                // And selected country is matched we should add this curriculum
                if (matchedCountries) {
                    let matchedCountry = matchedCountries.find(_ => !_.states || _.states.some(x => x.toLocaleLowerCase() == state));
                    if (matchedCountry) {
                        filteredCurricula.push(this.getContryRelatedCurriculum(c, matchedCountry));
                        return;
                    }
                    // If has empty contry name than include everywhere
                }
                if(c.availableCountries.some(_ => !_.name || _.name.length < 1)){
                    filteredCurricula.push(c);
                    return;
                }
            }

            // Add base(other) course if not added based on country
            let baseCurriculum = curricula.find(_ => _.curriculumCode == 0);
            let baseCountry = baseCurriculum?.availableCountries.find(_ => _.name == "");
            if (baseCurriculum && baseCountry && !filteredCurricula.some(_ => _.curriculumCode == 0)) {
                filteredCurricula.push(this.getContryRelatedCurriculum(baseCurriculum, baseCountry));
            }
        });

        filteredCurricula = filteredCurricula.sort((a, b) => b.orderPriority - a.orderPriority);

        return filteredCurricula;
    }

    getContryRelatedCurriculum(baseCurriculum: GetCurriculaWithCountryRelationQuery_Result, country: GetCurriculaWithCountryRelationQuery_Country_Result): GetCurriculaWithCountryRelationQuery_Result {
        baseCurriculum.name = country?.curriculumName ?? baseCurriculum.name;
        return baseCurriculum;
    }
}