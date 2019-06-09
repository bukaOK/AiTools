<template>
    <v-card>
        <v-card-title><h2 class="headline">Анализ продаж</h2></v-card-title>
        <v-card-text>
            <v-card>
                <v-card-title>
                    <span class="title">Параметры</span>
                    <v-spacer />
                    <v-btn color="primary" @click="addParam">Добавить параметр</v-btn>
                </v-card-title>
                <v-card-text>
                    <v-container grid-list-lg class="pa-0">
                        <v-form ref="form">
                            <input type="file" ref="parseFile" @change="fileUploaded" style="display: none" />
                            <v-layout row wrap>
                                <v-flex xs6 md3>
                                    <v-checkbox class="mt-0" label="У файла есть заголовок" color="primary" v-model="hasHeader" />
                                </v-flex>
                                <v-flex xs6 md3>
                                    <v-checkbox class="mt-0" label="Учитывать группы" color="primary" v-model="useGroups" />
                                </v-flex>
                            </v-layout>
                            <v-layout row wrap>
                                <v-flex xs12 md4 v-for="(par, ind) in params" :key="ind">
                                    <v-text-field box label="Название" v-model="par.name" />
                                    <v-text-field v-if="hasHeader" box label="Название колонки" v-model="par.colName" />
                                    <v-text-field v-else box label="Индекс" v-model="par.col" />
                                    <v-select solo label="Структура" :items="availableStructures" v-model="par.struct" />
                                    <v-select v-if="par.struct === 4" solo label="Операция" :items="availableComboOperations" v-model="par.operation" />
                                    <v-btn block color="red" dark @click="removeParam(ind)">Удалить параметр</v-btn>
                                </v-flex>
                            </v-layout>
                        </v-form>
                    </v-container>
                </v-card-text>
            </v-card>
            <v-card class="mt-3">
                <v-card-title>
                    <span class="title">Результаты</span>
                    <v-spacer />
                    <v-btn color="primary" @click="uploadClick" :loading="parseLoading">Загрузить файл для анализа</v-btn>
                    <v-btn color="success" v-if="charts && charts.length" @click="showSaveDialog = true">Сохранить</v-btn>
                </v-card-title>
                <v-card-text>
                    <v-form>
                        <h3 class="subtitle">Настройки графиков</h3>
                        <v-layout row wrap>
                            <v-flex xs6 md3>
                                <v-checkbox label="Учитывать месяц" color="primary" v-model="chartSettings.useMonth" />
                            </v-flex>
                            <v-flex xs6 md3>
                                <v-checkbox label="Учитывать день недели" color="primary" v-model="chartSettings.useWeekday" />
                            </v-flex>
                            <v-flex xs6 md3>
                                <v-checkbox label="Учитывать месяц и год" color="primary" v-model="chartSettings.useMonthYear" />
                            </v-flex>
                        </v-layout>
                    </v-form>
                    <div v-for="(chart, ind) in charts" :key="ind" class="mb-3">
                        <h3 class="subtitle">Данные за {{chart.header}}</h3>
                        <v-select max-height="300" :items="chartTypes" v-model="chart.chartType" />
                        <ai-bar v-if="chart.chartType === 'bar'" :datacollection="chart" style="position: relative; height: 50vh" />
                        <ai-line v-else :datacollection="chart" style="position: relative; height: 50vh" />
                    </div>
                </v-card-text>
            </v-card>
        </v-card-text>
        <v-dialog max-width="300" v-model="showSaveDialog">
            <v-card>
                <v-toolbar color="indigo" dark>
                    <v-toolbar-title>Сохранение анализа</v-toolbar-title>
                </v-toolbar>
                <v-card-text>
                    <v-form ref="dialogForm">
                        <v-text-field label="Название" v-model="analyzeName" :rules="rules.analyzeName" />
                        <v-alert v-for="(error, ind) in errors" type="error" :key="ind">
                            {{error}}
                        </v-alert>
                    </v-form>
                </v-card-text>
                <v-card-actions>
                    <v-btn flat color="success" @click="save">Сохранить</v-btn>
                </v-card-actions>
            </v-card>
        </v-dialog>
        <v-snackbar v-model="showSnack" color="error">{{serverError}}</v-snackbar>
    </v-card>
</template>
<script>
import AiBar from '../components/AiBar'
import AiLine from '../components/AiLine'
import Papa from 'papaparse'
import { mapState } from 'vuex'
import colorMixin from '../mixins/colorMixin'

export default {
    props: ['id'],
    mixins: [colorMixin],
    components: {
        AiBar,
        AiLine
    },
    mounted(){
        if(this.id){
            this.api.get('/analyze/getdata/' + this.id).then(resp => {
                this.analyzeName = resp.data.name
                const data = JSON.parse(resp.data.data)
                this.params = data.params
                this.chartSettings = data.chartSettings
                this.charts = data.charts.map(chart => ({
                        chartType: chart.chartType,
                        header: chart.header,
                        labels: chart.labels,
                        datasets: chart.datasets.map(ds => ({
                            backgroundColor: ds.backgroundColor,
                            borderColor: ds.borderColor,
                            data: ds.data,
                            label: ds.label,
                        }))
                    }))
            })
        }
    },
    data(){
        return {
            hasHeader: true,
            useGroups: false,
            parseLoading: false,
            analyzeName: '',
            chartSettings: {
                useMonth: true,
                useWeekday: false,
                useMonthYear: true
            },
            params: [
                {
                    col: 1,
                    struct: 0,
                    name: 'Дата',
                    colName: 'date',
                    operation: null
                },
                {
                    col: 2,
                    struct: 1,
                    name: 'Количество продаж',
                    colName: 'sales',
                    operation: null
                },
                {
                    col: 3,
                    struct: 2,
                    name: 'Магазин',
                    colName: 'store',
                    operation: null,
                }
            ],
            monthLabels: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"],
            weekLabels: ['Воскресенье', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота'],
            charts: [],
            rules: {
                analyzeName: [
                    v => !!v || 'Заполните название анализа'
                ]
            },
            errors: [],
            serverError: '',
            showSaveDialog: false,
            showSnack: false
        }
    },
    computed: {
        availableStructures(){
            return ['Дата', 'Продажи', 'Группа', 'Комбо'].map((x, ind) => ({
                    text: x,
                    value: ind
                }))
        },
        availableComboOperations(){
            return ['Умножение', 'Сложение'].map((x, ind) => ({
                    text: x,
                    value: ind
                }))
        },
        chartTypes(){
            return Object.entries({
                'line': 'Линейный график',
                'bar': 'Столбчатая диаграмма'
            }).map(x => ({
                text: x[1],
                value: x[0]
            }))
        },
        ...mapState([
            'api'
        ])
    },
    methods: {
        addParam(){
            const maxColNum = Math.max.apply(null, this.params.map(x => x.col))
            this.params.push({
                col: maxColNum + 1,
                struct: 1,
                name: '',
                operation: null,
                colName: null
            })
        },
        removeParam(index){
            this.params.splice(index, 1)
        },
        uploadClick(){
            this.$refs.parseFile.click()
        },
        fileUploaded(){
            const file = this.$refs.parseFile.files[0]
            const data = {}
            const dateParam = this.params.find(x => x.struct === 0)
            const salesParam = this.params.find(x => x.struct === 1)
            const salesKey = this.hasHeader ? salesParam.colName : salesParam.col - 1

            for(let par of this.params.filter(x => x.struct === 2)){
                const ind = par.colName ? par.colName : par.col - 1
                data[ind] = {}
            }
            this.parseLoading = true

            Papa.parse(file, {
                worker: true,
                delimiter: ',',
                header: true,
                skipEmptyLines: true,
                step: (result) => {
                    const row = result.data
                    const dateStr = this.hasHeader ? row[dateParam.colName] : row[dateParam.index]
                    const date = new Date(dateStr)
                    const month = date.getMonth()
                    const weekDay = date.getDay()
                    const monthYear = `${date.getFullYear()}-${month}`
                    
                    for(let key in data){
                        if(!data[key][row[key]]){
                            data[key][row[key]] = {}
                        }
                        if(!data[key][row[key]][`month-${month}`]) {
                            data[key][row[key]][`month-${month}`] = +row[salesKey]
                        } else{
                            data[key][row[key]][`month-${month}`] += +row[salesKey]
                        }
                        if(!data[key][row[key]][`weekday-${weekDay}`]){
                            data[key][row[key]][`weekday-${weekDay}`] = +row[salesKey]
                        } else{
                            data[key][row[key]][`weekday-${weekDay}`] += +row[salesKey]
                        }
                        if(!data[key][row[key]][`monthyear-${monthYear}`]){
                            data[key][row[key]][`monthyear-${monthYear}`] = +row[salesKey]
                        } else{
                            data[key][row[key]][`monthyear-${monthYear}`] += +row[salesKey]
                        }
                    }
                },
                complete: () => {
                    this.charts = []
                    this.parseLoading = false
                    this.mapData(data)
                }
            })
        },
        mapData(data){
            for(let key in data){
                const param = this.params.find(x => x.colName === key || x.index === key)
                const datasets = []
                if(this.chartSettings.useMonth){
                    const monthDatasets = this.getDatasets(data[key], 'month', param)
                    this.charts.push({
                        header: 'месяц',
                        chartType: 'bar',
                        labels: this.monthLabels,
                        datasets: monthDatasets
                    })
                }
                if(this.chartSettings.useMonthYear){
                    const monthYearDatasets = this.getDatasets(data[key], 'monthyear', param)
                    const firstItem = Object.entries(data[key])[0][1]
                    const monthYearLabels = []
                    for(let src of Object.keys(firstItem)){
                        const srcArr = src.split('-')
                        if(srcArr[0] === 'monthyear'){
                            const year = srcArr[1]
                            const month = srcArr[2]
                            monthYearLabels.push(`${this.monthLabels[month]} ${year}`)
                        }
                    }
                    this.charts.push({
                        header: 'месяц и год',
                        chartType: 'line',
                        labels: monthYearLabels,
                        datasets: monthYearDatasets
                    })
                }
            }
        },
        getDatasets(itemData, intervalLabel, param){
            const intervalDatasets = []
            if(this.useGroups){
                for(let itemKey in itemData){
                    const itemData = Object.entries(itemData[itemKey])
                        .filter(item => item[0].split('-')[0] === intervalLabel)
                        .map(item => item[1])
                    intervalDatasets.push({
                        label: `${param.name} ${itemKey}`,
                        data: itemData,
                        backgroundColor: this.getRandomColor()
                    })
                }
            } else{
                const intervalData = []
                for(let itemKey in itemData){
                    let i = 0;
                    for(let intervalKey in itemData[itemKey]){
                        if(intervalKey.split('-')[0] === intervalLabel){
                            const sales = itemData[itemKey][intervalKey]
                            if(!intervalData[i]){
                                intervalData[i] = sales
                            } else {
                                intervalData[i] += sales
                            }
                            i++
                        }
                    }
                }
                intervalDatasets.push({
                    label: param.name,
                    data: intervalData,
                    backgroundColor: this.getRandomColor()
                })
            }
            return intervalDatasets
        },
        save(){
            if(!this.$refs.dialogForm.validate()){
                return
            }
            if(!this.charts || !this.charts.length){
                this.errors.push('Данные не заполнены')
                return
            }
            const data = {
                charts: this.charts.map(chart => {
                    return {
                        chartType: chart.chartType,
                        header: chart.header,
                        labels: chart.labels,
                        datasets: chart.datasets.map(ds => ({
                            backgroundColor: ds.backgroundColor,
                            borderColor: ds.borderColor,
                            data: ds.data,
                            label: ds.label,
                        }))
                    }
                }),
                params: this.params,
                chartSettings: this.chartSettings
            }
            const dataStr = JSON.stringify(data)
            const fd = new FormData()
            console.log(data)
            fd.append('Name', this.analyzeName)
            fd.append('Data', dataStr)
            this.api.post('/analyze/add', fd).then(() => {
                this.$route.push('/')
            }).catch(err => {
                this.serverError = 'Внутренняя ошибка'
                this.showSnack = true
            })
        }
    }
}
</script>
