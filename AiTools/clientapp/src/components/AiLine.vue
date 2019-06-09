<script>
import {Line} from 'vue-chartjs'

export default {
    extends: Line,
    props: ['datacollection'],
    data: () => ({
        options: {
            responsive: true,
            maintainAspectRatio: false
        }
    }),
    mounted(){
        for(let item of this.datacollection.datasets){
            const rgb = item.backgroundColor.substring(5, item.backgroundColor.length-1).replace(/ /g, '').split(',')
            item.borderColor = `rgb(${rgb[0]}, ${rgb[1]}, ${rgb[2]})`
        }
        console.log(this.datacollection)
        this.renderChart(this.datacollection, this.options)
    }
}
</script>
