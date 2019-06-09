<template>
<v-container grid-list-lg class="pa-0">
    <v-layout row wrap>
        <v-flex xs12 md8>
            <v-card>
                <v-card-title class="headline">Обучение модели</v-card-title>
                <v-container grid-list-lg class="pt-0">
                    <v-layout row wrap>
                        <v-flex xs6 md4>
                            <v-card>
                                <v-card-title>
                                    <div>
                                        <div class="subheading">Данные для обучения</div>
                                        <v-divider v-if="trainFilename" />
                                        <div>{{trainFilename}}</div>
                                    </div>
                                </v-card-title>
                                <v-card-actions>
                                    <v-btn block flat color="primary" @click="chooseFile('trainFile')">Выбрать</v-btn>
                                </v-card-actions>
                            </v-card>
                        </v-flex>
                        <v-flex xs6 md8>
                            <v-card>
                                <v-card-title class="subheading">Параметры</v-card-title>
                                <v-card-text class="pt-0 pb-0">
                                    <form>
                                        <v-layout>
                                            <v-flex md6>
                                                <v-text-field label="Название" v-model="name" :rules="rules.name" />
                                                <v-checkbox label="Учитывать день недели" v-model="useWeekday" color="primary" />
                                                <v-checkbox label="Учитывать день месяца" v-model="useMonthDay" color="primary" />
                                            </v-flex>
                                            <v-flex md6>
                                                <v-text-field label="Разделитель" v-model="delimiter" :rules="rules.delimiter" />
                                                <v-checkbox label="Учитывать месяц" v-model="useMonth" color="primary" />
                                                <v-checkbox label="Учитывать год" v-model="useYear" color="primary" />
                                            </v-flex>
                                        </v-layout>
                                    </form>
                                </v-card-text>
                            </v-card>
                        </v-flex>
                    </v-layout>
                    <v-alert v-for="error in errors" :key="error">{{error}}</v-alert>
                    <form>
                        <input @change="fileChange('trainFile')" type="file" ref="trainFile" style="display: none" />
                        <input @change="fileChange('evalFile')" type="file" ref="evalFile" style="display: none" />
                    </form>
                </v-container>
                <v-card-actions>
                    <v-btn color="indigo" @click="train" dark block large :loading="loading">Обучить</v-btn>
                </v-card-actions>
            </v-card>
        </v-flex>
        <v-flex xs12 md4>
            <v-card v-if="id">
                <v-card-title class="headline">Составление прогноза</v-card-title>
                <v-card-text>
                    <v-form>
                        <v-card>
                            <v-card-title>
                                <div>
                                    <div class="subheading">Данные для прогноза</div>
                                    <v-divider v-if="evalFilename" />
                                    <div>{{evalFilename}}</div>
                                </div>
                            </v-card-title>
                            <v-card-actions>
                                <v-btn block flat color="primary" @click="chooseFile('evalFile')">Выбрать</v-btn>
                            </v-card-actions>
                        </v-card>
                        <v-text-field v-model="evalDelimiter" label="Разделитель" class="mt-3" />
                    </v-form>
                </v-card-text>
                <v-card-actions>
                    <v-btn block color="primary" dark @click="predict" :loading="evalLoading">Составить прогноз</v-btn>
                </v-card-actions>
            </v-card>
        </v-flex>
    </v-layout>
    <v-card v-if="forecastChart.labels.length">
        <v-card-title class="headline">Результаты прогноза</v-card-title>
        <v-card-text>
            <ai-line :datacollection="forecastChart" />
        </v-card-text>
    </v-card>
</v-container>
</template>
<script>
import { mapState } from 'vuex';
import Papa from 'papaparse'
import colorMixin from '../mixins/colorMixin'
import AiLine from '../components/AiLine'

export default {
    mixins: [colorMixin],
    components: {AiLine},
    mounted(){
        if(this.id){
            this.api.get('/forecast/getforedit/' + this.id).then(resp => {
                this.name = resp.data.name
            })
        }
    },
    data(){
        return {
            id: this.$route.params.id,
            errors: [],
            trainFilename: '',
            evalFilename: '',
            loading: false,
            evalLoading: false,
            trainName: '',
            name: '',
            delimiter: '',
            evalDelimiter: '',
            useWeekday: true,
            useMonthDay: false,
            useYear: true,
            useMonth: true,
            forecastChart: {
                header: 'месяц',
                chartType: 'line',
                labels: [],
                datasets: []
            },
            monthLabels: ['Январь', 'Февраль', 'Март', 'Апрель', 'Май', 'Июнь', 'Июль', "Август", "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"],
            rules: {
                name: [
                    v => !!v || 'Заполните название'
                ],
                delimiter: [
                    v => !!v || 'Заполните разделитель',
                    v => /^\\?.?$/.test(v) || 'Длина не должна быть больше 1'
                ]
            }
        }
    },
    computed: {
        ...mapState([
            'api'
        ])
    },
    methods:{
        chooseFile(inputRef){
            this.$refs[inputRef].click()
        },
        fileChange(type){
            this[type + 'name'] = this.$refs[type].files[0].name
        },
        train(){
            this.errors = []
            let trainFile = this.$refs.trainFile.files[0]
            if(!trainFile){
                this.errors.push('Выберите файл для обучения')
            } else {
                const trainFormatArr = trainFile.name.split('.')
                
                if(trainFormatArr[trainFormatArr.length - 1] !== 'csv'){
                    this.errors.push('Формат файлов должен быть csv')
                } else{
                    const fd = new FormData()
                    fd.append('File', trainFile)
                    fd.append('Name', this.name)
                    fd.append('Delimiter', this.delimiter)
                    this.loading = true
                    this.api.post('/forecast/train', fd).then(resp => {
                        console.log(resp)
                    }).finally(() => {
                        this.loading = false
                    })
                }
            }
        },
        predict(){
            if(this.id){
                this.errors = []
                let evalFile = this.$refs.evalFile.files[0]

                if(!evalFile){
                    this.errors.push('Выберите файл для обучения')
                } else {
                    const evalFormatArr = evalFile.name.split('.')
                    
                    if(evalFormatArr[evalFormatArr.length - 1] !== 'csv'){
                        this.errors.push('Формат файлов должен быть csv')
                    } else{
                        const fd = new FormData()
                        fd.append('File', evalFile)
                        fd.append('Delimiter', this.evalDelimiter)
                        fd.append('Id', this.id)
                        this.evalLoading = true
                        this.api.post('/forecast/predict', fd).then(resp => {
                            const data = []
                            const labels = []

                            Papa.parse(evalFile, {
                                worker: true,
                                delimiter: ',',
                                header: true,
                                skipEmptyLines: true,
                                step: result => {
                                    const row = result.data
                                    const date = new Date(row.date)
                                    const monthYearKey = `${date.getFullYear()}-${date.getMonth()}`
                                    const monthYearLabel = `${this.monthLabels[date.getMonth()]} ${date.getFullYear()}`

                                    if(!data[row.store]){
                                        data[row.store] = {}
                                    }
                                    if(!data[row.store][monthYearKey]){
                                        data[row.store][monthYearKey] = 0
                                    }
                                    if(!labels.includes(monthYearLabel)){
                                        labels.push(monthYearLabel)
                                    }
                                    const sales = resp.data.find(x => x.id === row.id).sales
                                    data[row.store][monthYearKey] += sales
                                },
                                complete: () => {
                                    console.log(data)
                                    this.forecastChart.labels = labels
                                    this.forecastChart.datasets = []

                                    for(let storeKey in data){
                                        this.forecastChart.datasets.push({
                                            label: 'Магазин ' + storeKey,
                                            backgroundColor: this.getRandomColor(),
                                            data: Object.keys(data[storeKey]).sort().map(key => data[storeKey][key])
                                        })
                                    }
                                }
                            })
                        }).finally(() => {
                            this.evalLoading = false
                        })
                    }
                }
            }
        }
    }
}
</script>
